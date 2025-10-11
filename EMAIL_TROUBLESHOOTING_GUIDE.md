# ?? EMAIL TROUBLESHOOTING CHECKLIST

## ?? Problem: Email not received at narasimhasez@gmail.com

Let's check each step systematically:

---

## ? **STEP 1: Verify SMTP Configuration in Azure**

### Go to Azure Portal:
1. Open **Azure Portal** ? Your App Service
2. Click **Configuration** (under Settings)
3. Look for these settings under **Application settings**:

**Required Settings:**
```
Name: Notifications__Smtp__Host
Value: smtp.gmail.com

Name: Notifications__Smtp__Port
Value: 587

Name: Notifications__Smtp__From
Value: YOUR_SENDER_EMAIL@gmail.com

Name: Notifications__Smtp__User
Value: YOUR_SENDER_EMAIL@gmail.com

Name: Notifications__Smtp__Pass
Value: YOUR_GMAIL_APP_PASSWORD
```

### ? Are all 5 settings present?
- [ ] YES ? Go to Step 2
- [ ] NO ? **ADD THEM NOW** (see configuration section below)

---

## ? **STEP 2: Check Azure Logs**

### View Live Logs:
1. Azure Portal ? Your App Service
2. Click **Log stream** (under Monitoring)
3. Wait for logs to load
4. Look for these specific messages:

### What to Look For:

#### **Scenario A: SMTP Not Configured**
```
[Information] Email not sent: SMTP not configured (Notifications:Smtp:Host/From)
```
**? Problem:** SMTP settings missing in Azure
**? Solution:** Add configuration settings (see Step 5)

#### **Scenario B: No Matching Blood Banks**
```
[Information] Blood request created by nawya: A+, 2 units. Found 0 matching blood banks
```
**? Problem:** No blood banks found with sufficient stock
**? Solution:** Verify blood bank data (see Step 3)

#### **Scenario C: Blood Banks Found, Emails Sent**
```
[Information] Blood request created by nawya: A+, 2 units. Found 1 matching blood banks
[Information] Notification sent to blood bank: Narasimha Blood Bank (narasimhasez@gmail.com)
[Information] Email sent to narasimhasez@gmail.com
```
**? Success:** Email was sent! Check spam folder (see Step 4)

#### **Scenario D: Email Send Failed**
```
[Warning] Failed to send notification to blood bank Narasimha Blood Bank
[Warning] Email send failed to narasimhasez@gmail.com
```
**? Problem:** SMTP credentials incorrect
**? Solution:** Verify Gmail App Password (see Step 5)

### ? What do you see in the logs?
Write down the exact message you see.

---

## ? **STEP 3: Verify Blood Bank Data in Database**

Run this SQL query in **Azure SQL Query Editor**:

```sql
-- Check if Narasimha Blood Bank exists
SELECT 
    h.Id,
    h.Name,
    h.Email,
    h.IsActive,
    bi.APos AS [A+ Stock],
    bi.BPos AS [B+ Stock],
    bi.OPos AS [O+ Stock]
FROM [dbo].[Hospitals] h
LEFT JOIN [dbo].[BloodInventories] bi ON h.Id = bi.HospitalId
WHERE h.Email = 'narasimhasez@gmail.com'
```

### Expected Result:
```
Id: (some number)
Name: Narasimha Blood Bank
Email: narasimhasez@gmail.com
IsActive: 1 (or True)
A+ Stock: 50
B+ Stock: 40
O+ Stock: 60
```

### ? What do you get?

**If NO ROWS returned:**
- **Problem:** Blood bank not in database
- **Solution:** Run the SQL insert script I provided earlier

**If IsActive = 0 (or False):**
- **Problem:** Blood bank is inactive
- **Solution:** Run this:
```sql
UPDATE [dbo].[Hospitals] 
SET IsActive = 1 
WHERE Email = 'narasimhasez@gmail.com'
```

**If Stock values are NULL or 0:**
- **Problem:** No blood inventory
- **Solution:** Run the blood inventory insert script

**If Email is NULL:**
- **Problem:** Email not set
- **Solution:** Run this:
```sql
UPDATE [dbo].[Hospitals] 
SET Email = 'narasimhasez@gmail.com' 
WHERE Name = 'Narasimha Blood Bank'
```

---

## ? **STEP 4: Check Blood Request Details**

Run this SQL query:

```sql
-- Check your most recent blood request
SELECT TOP 1
    br.Id,
    br.BloodGroup,
    br.Units,
    br.PersonName,
    br.Status,
    br.RequestedAt,
    u.Username AS RequestingHospital,
    u.Email AS HospitalEmail
FROM [dbo].[BloodRequests] br
LEFT JOIN [dbo].[Users] u ON u.Username = (SELECT TOP 1 Username FROM Users WHERE Role = 'User')
ORDER BY br.RequestedAt DESC
```

### ? What blood group did you request?
- If you requested **A+** and Narasimha has A+ stock ? Should work ?
- If you requested **O-** and Narasimha has no O- stock ? Won't notify ?

### Matching Logic:
```
Your Request: A+, 2 units
Narasimha Stock: A+ = 50 units

Match? 50 >= 2 ? YES ?
Email should be sent
```

---

## ? **STEP 5: Configure SMTP in Azure (If Missing)**

### Step-by-Step:

1. **Go to Azure Portal**
2. **Open your App Service**
3. **Click Configuration** (under Settings)
4. **Click "+ New application setting"**
5. **Add these ONE BY ONE:**

#### Setting 1:
```
Name: Notifications__Smtp__Host
Value: smtp.gmail.com
```
Click OK

#### Setting 2:
```
Name: Notifications__Smtp__Port
Value: 587
```
Click OK

#### Setting 3:
```
Name: Notifications__Smtp__From
Value: narasimhasez@gmail.com
```
Click OK (or use your own Gmail)

#### Setting 4:
```
Name: Notifications__Smtp__User
Value: narasimhasez@gmail.com
```
Click OK (same as From)

#### Setting 5:
```
Name: Notifications__Smtp__Pass
Value: YOUR_GMAIL_APP_PASSWORD_HERE
```
Click OK

**?? IMPORTANT:** This must be a Gmail **App Password**, not your regular password!

### How to Get Gmail App Password:

1. Go to https://myaccount.google.com/security
2. Enable **2-Step Verification** (if not already)
3. Search for "App passwords"
4. Click "App passwords"
5. Select **Mail** and **Other (Custom name)**
6. Type "BloodHub Azure"
7. Click **Generate**
8. **COPY THE 16-CHARACTER PASSWORD** (no spaces)
9. Use this in `Notifications__Smtp__Pass`

### After Adding All Settings:

6. **Click "Save"** at the top
7. **Click "Continue"** on the confirmation dialog
8. **RESTART the app** (click Restart button at top)
9. **Wait 1-2 minutes** for app to fully restart

---

## ? **STEP 6: Test Again**

### After Configuration:

1. **Login to your app** as user "nawya"
2. **Create a NEW blood request**:
   - Blood Group: **A+**
   - Units: **2**
   - Patient Name: Test Patient
   - Mobile: 9876543210
   - Address: Test Address
3. **Click "Save Request"**
4. **Wait 30 seconds**
5. **Check narasimhasez@gmail.com**

---

## ? **STEP 7: Check Email Spam Folder**

If email was sent but you don't see it:

1. Login to **narasimhasez@gmail.com**
2. Check **Spam** folder
3. Look for email from your sender address
4. Subject: "?? Blood Request: A+ (2 units) - From nawya"

If found in spam:
- Click "Not spam"
- Move to inbox

---

## ?? **Quick Diagnostic SQL Queries**

### Query 1: Check All Blood Banks
```sql
SELECT 
    h.Name,
    h.Email,
    h.IsActive,
    CASE WHEN bi.Id IS NULL THEN 'NO STOCK' ELSE 'HAS STOCK' END AS StockStatus
FROM [dbo].[Hospitals] h
LEFT JOIN [dbo].[BloodInventories] bi ON h.Id = bi.HospitalId
WHERE h.Email IS NOT NULL
```

### Query 2: Check What Would Match Your Request
```sql
-- Replace 'A+' and '2' with your actual request
DECLARE @BloodGroup VARCHAR(10) = 'A+'
DECLARE @Units INT = 2

SELECT 
    h.Name,
    h.Email,
    CASE @BloodGroup
        WHEN 'A+' THEN bi.APos
        WHEN 'A-' THEN bi.ANeg
        WHEN 'B+' THEN bi.BPos
        WHEN 'B-' THEN bi.BNeg
        WHEN 'O+' THEN bi.OPos
        WHEN 'O-' THEN bi.ONeg
        WHEN 'AB+' THEN bi.ABPos
        WHEN 'AB-' THEN bi.ABNeg
    END AS AvailableUnits,
    CASE 
        WHEN CASE @BloodGroup
            WHEN 'A+' THEN bi.APos
            WHEN 'A-' THEN bi.ANeg
            WHEN 'B+' THEN bi.BPos
            WHEN 'B-' THEN bi.BNeg
            WHEN 'O+' THEN bi.OPos
            WHEN 'O-' THEN bi.ONeg
            WHEN 'AB+' THEN bi.ABPos
            WHEN 'AB-' THEN bi.ABNeg
        END >= @Units THEN 'MATCH ?'
        ELSE 'NO MATCH ?'
    END AS WouldNotify
FROM [dbo].[Hospitals] h
LEFT JOIN [dbo].[BloodInventories] bi ON h.Id = bi.HospitalId
WHERE h.Email = 'narasimhasez@gmail.com'
```

---

## ?? **Most Common Issues:**

### Issue 1: SMTP Not Configured
**Symptom:** Logs show "Email not sent: SMTP not configured"
**Solution:** Add all 5 SMTP settings in Azure Configuration

### Issue 2: Wrong Blood Group
**Symptom:** Logs show "Found 0 matching blood banks"
**Solution:** Verify blood bank has stock for requested blood group

### Issue 3: Wrong Gmail Password
**Symptom:** Logs show "Authentication failed"
**Solution:** Use App Password (not regular password)

### Issue 4: App Not Restarted
**Symptom:** Changes not taking effect
**Solution:** Restart app after configuration changes

### Issue 5: Email in Spam
**Symptom:** Email sent but not in inbox
**Solution:** Check spam folder

---

## ?? **What to Tell Me:**

Please check and tell me:

1. **Do you see SMTP settings in Azure Configuration?**
   - [ ] Yes, all 5 settings present
   - [ ] No, settings missing
   - [ ] Not sure how to check

2. **What do Azure logs show?**
   - Copy the exact log message you see

3. **What does the blood bank SQL query return?**
   - Copy the results

4. **What blood group did you request?**
   - A+, B+, O+, etc.

5. **Did you check spam folder?**
   - [ ] Yes, checked - nothing there
   - [ ] Not yet

---

## ?? **Next Steps:**

Once you provide the information above, I can tell you exactly what's wrong and how to fix it!
