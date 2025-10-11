# ?? Azure App Service Configuration - Quick Setup

## Copy-Paste Ready Configuration

### Step 1: Get Gmail App Password

1. Go to: https://myaccount.google.com/security
2. Enable **2-Step Verification**
3. Search for "App passwords"
4. Generate password for "BloodHub"
5. Copy the 16-character code (remove spaces)

---

### Step 2: Add to Azure App Service

**Navigate to:**
```
Azure Portal ? Your App Service ? Configuration ? Application Settings
```

**Click "+ New application setting" and add these ONE BY ONE:**

---

#### Setting 1: SMTP Host
```
Name: Notifications__Smtp__Host
Value: smtp.gmail.com
```

#### Setting 2: SMTP Port
```
Name: Notifications__Smtp__Port
Value: 587
```

#### Setting 3: From Email
```
Name: Notifications__Smtp__From
Value: YOUR_GMAIL_ADDRESS@gmail.com
```
?? Replace with your actual Gmail address

#### Setting 4: SMTP User
```
Name: Notifications__Smtp__User
Value: YOUR_GMAIL_ADDRESS@gmail.com
```
?? Replace with your actual Gmail address

#### Setting 5: SMTP Password
```
Name: Notifications__Smtp__Pass
Value: YOUR_16_CHAR_APP_PASSWORD
```
?? Replace with the App Password from Step 1

---

### Step 3: Save & Restart

1. Click **Save** (top of page)
2. Click **OK** on the confirmation dialog
3. Click **Restart** (top of page)
4. Wait 30 seconds for app to restart

---

## Verification

### Check if Settings Applied:

**PowerShell (Azure CLI):**
```powershell
az webapp config appsettings list --name YOUR_APP_NAME --resource-group YOUR_RESOURCE_GROUP
```

**Expected Output:**
```json
{
  "name": "Notifications__Smtp__Host",
  "value": "smtp.gmail.com"
},
{
  "name": "Notifications__Smtp__Port",
  "value": "587"
},
...
```

---

## Test Notification

### Step 1: Add Test Donor

1. Login to your Azure-deployed application
2. Go to **Donor Service**
3. Add donor:
   - Name: Test User
   - Email: **YOUR_EMAIL@gmail.com** (use your email for testing)
   - Blood Group: **A+**
   - Service Type: **Free**
   - Fill other required fields
4. Click Submit

### Step 2: Create Blood Request

1. Go to **Requests** page
2. Click **+ New Request**
3. Fill form:
   - Hospital: Any
   - Blood Group: **A+** (must match donor's blood group)
   - Units: 1
   - Patient Name: Test Patient
   - Mobile: 1234567890
   - Address: Test Address
4. Click **Save Request**

### Step 3: Check Your Email

- ? You should receive an email within 10-30 seconds
- ? Subject: "?? Urgent: A+ Blood Needed at [Hospital Name]"
- ? Contains all request details

---

## Troubleshooting

### Issue 1: No Email Received

**Check 1: Are settings correct?**
```
Azure Portal ? App Service ? Configuration ? Application Settings
Verify all 5 settings exist with correct names (double underscore __)
```

**Check 2: Check Logs**
```
Azure Portal ? App Service ? Log stream
Look for:
- "Blood request created: A+ at Hospital. Found X matching donors"
- "Email sent to your@email.com"
```

**Check 3: Gmail App Password Valid?**
- Remove spaces from App Password
- Must be 16 characters
- Not your regular Gmail password

**Check 4: Spam Folder**
- Check your spam/junk folder
- Mark as "Not Spam" if found there

---

### Issue 2: "SMTP not configured" in Logs

**Cause:** Setting names incorrect (wrong underscores)

**Fix:** Settings must use `__` (double underscore), not single `_`

? Wrong: `Notifications_Smtp_Host`  
? Correct: `Notifications__Smtp__Host`

---

### Issue 3: "Authentication failed"

**Cause:** Incorrect App Password

**Fix:**
1. Delete old App Password in Google Account
2. Generate new one
3. Update `Notifications__Smtp__Pass` in Azure
4. Restart app

---

## Alternative Email Providers

### Using SendGrid (Azure Free Tier):

```
Name: Notifications__Smtp__Host
Value: smtp.sendgrid.net

Name: Notifications__Smtp__Port
Value: 587

Name: Notifications__Smtp__From
Value: noreply@yourdomain.com

Name: Notifications__Smtp__User
Value: apikey

Name: Notifications__Smtp__Pass
Value: YOUR_SENDGRID_API_KEY
```

### Using Outlook:

```
Name: Notifications__Smtp__Host
Value: smtp-mail.outlook.com

Name: Notifications__Smtp__Port
Value: 587

Name: Notifications__Smtp__From
Value: your@outlook.com

Name: Notifications__Smtp__User
Value: your@outlook.com

Name: Notifications__Smtp__Pass
Value: YOUR_OUTLOOK_PASSWORD
```

---

## Security Best Practices

### ? DO:
- Use App-Specific Passwords
- Enable 2-Factor Authentication
- Use environment variables (App Settings)
- Monitor logs regularly

### ? DON'T:
- Commit passwords to Git
- Use regular Gmail password
- Share App Passwords
- Store passwords in appsettings.json

---

## Monitoring

### Enable Application Insights (Recommended):

1. Azure Portal ? Your App Service
2. Application Insights ? Enable
3. View telemetry:
   - Request counts
   - Failed requests
   - Dependencies (SMTP calls)

### Key Metrics to Monitor:

- **Total Notifications Sent**: Check logs for "Email sent to..."
- **Failed Notifications**: Check logs for "Email send failed..."
- **Matching Donors Found**: Check logs for "Found X matching donors"

---

## FAQ

**Q: Will this work in India?**  
A: Yes! Gmail SMTP works globally, including India.

**Q: How many emails can I send per day?**  
A: Gmail allows ~500 emails/day for free accounts, 2000/day for Google Workspace.

**Q: What if I exceed the limit?**  
A: Consider SendGrid (12,000 emails/month free) or Amazon SES.

**Q: Can I use custom domain email?**  
A: Yes! Configure your email provider's SMTP settings accordingly.

**Q: Is this production-ready?**  
A: Yes! This setup is suitable for production use.

---

## Quick Commands

### View Logs:
```bash
az webapp log tail --name YOUR_APP_NAME --resource-group YOUR_RESOURCE_GROUP
```

### Restart App:
```bash
az webapp restart --name YOUR_APP_NAME --resource-group YOUR_RESOURCE_GROUP
```

### Update Setting:
```bash
az webapp config appsettings set --name YOUR_APP_NAME --resource-group YOUR_RESOURCE_GROUP --settings "Notifications__Smtp__Pass=NEW_PASSWORD"
```

---

## Summary Checklist

- [ ] Gmail 2-Step Verification enabled
- [ ] App Password generated (16 characters)
- [ ] 5 Azure App Settings added (with double __)
- [ ] App restarted after configuration
- [ ] Test donor added with your email
- [ ] Test blood request created
- [ ] Email received successfully
- [ ] Checked logs for confirmation

---

**Status After Setup**: ? **PRODUCTION READY**

Your BloodHub application will now automatically notify all matching blood group donors whenever a hospital creates a blood request!
