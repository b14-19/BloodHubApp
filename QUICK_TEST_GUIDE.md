# ?? QUICK TROUBLESHOOTING STEPS

## ? I just created a Test Controller for you!

### **Step 1: Deploy the Updated Code to Azure**

```bash
git add .
git commit -m "Add test controller for troubleshooting"
git push origin master
```

Wait 2-3 minutes for Azure to deploy.

---

### **Step 2: Access the Verification Page**

Open your browser and go to:

```
https://your-app-name.azurewebsites.net/Test/Verify
```

**Replace `your-app-name` with your actual Azure app name!**

Example:
```
https://bloodhubapp.azurewebsites.net/Test/Verify
```

This page will show you:
- ? SMTP configuration status
- ? Blood banks in database
- ? Narasimha Blood Bank status
- ? Recent blood requests
- ? Email test results

---

### **Step 3: Send a Test Email**

Go to:
```
https://your-app-name.azurewebsites.net/Test/SendTestEmail
```

This will immediately send a test email to `narasimhasez@gmail.com`.

---

## ?? **What You'll See:**

### **Scenario A: SMTP Not Configured**
```
1. SMTP CONFIGURATION:
   Host: NOT SET
   Port: NOT SET
   From: NOT SET
   User: NOT SET
   Pass: NOT SET

6. SUMMARY:
   SMTP Configured: False
   
   ACTION REQUIRED:
   - Configure SMTP in Azure App Service
```

**Fix:** Add SMTP settings in Azure (see below)

---

### **Scenario B: Blood Bank Not Found**
```
3. NARASIMHA BLOOD BANK:
   NOT FOUND IN DATABASE
   ACTION: Run the SQL insert script
```

**Fix:** Run the SQL script I provided earlier

---

### **Scenario C: Everything Works!**
```
1. SMTP CONFIGURATION:
   Host: smtp.gmail.com
   Port: 587
   From: your-email@gmail.com
   User: your-email@gmail.com
   Pass: ****************

3. NARASIMHA BLOOD BANK:
   Found: Narasimha Blood Bank
   Email: narasimhasez@gmail.com
   Active: True
   Stock: A+=50, B+=40, O+=60...

5. EMAIL TEST:
   EMAIL SENT SUCCESSFULLY

6. SUMMARY:
   SYSTEM READY!
```

**? Perfect! Everything is working!**

---

## ?? **How to Configure SMTP in Azure:**

1. **Azure Portal** ? Your App Service
2. **Configuration** ? **Application settings**
3. **Click "+ New application setting"**

Add these 5 settings:

```
Name: Notifications__Smtp__Host
Value: smtp.gmail.com
```

```
Name: Notifications__Smtp__Port
Value: 587
```

```
Name: Notifications__Smtp__From
Value: narasimhasez@gmail.com
```

```
Name: Notifications__Smtp__User
Value: narasimhasez@gmail.com
```

```
Name: Notifications__Smtp__Pass
Value: YOUR_GMAIL_APP_PASSWORD
```

**Click "Save" and "Restart"** the app.

---

## ?? **How to Get Gmail App Password:**

1. Go to: https://myaccount.google.com/security
2. Enable **2-Step Verification**
3. Search "App passwords"
4. Create password for "Mail"
5. Copy the 16-character password
6. Use it in `Notifications__Smtp__Pass`

---

## ?? **Quick Commands:**

### Deploy to Azure:
```bash
cd BloodHubApp
git add .
git commit -m "Add test endpoints"
git push
```

### Test URLs:
```
Verification: https://YOUR-APP.azurewebsites.net/Test/Verify
Send Test Email: https://YOUR-APP.azurewebsites.net/Test/SendTestEmail
```

---

## ?? **After You Run /Test/Verify:**

**Copy the entire output and send it to me!**

I'll tell you exactly what's wrong and how to fix it.

---

## ?? **Quick Summary:**

1. ? **Deploy updated code** (with git push)
2. ? **Visit /Test/Verify** to see diagnostics
3. ? **Visit /Test/SendTestEmail** to test email
4. ? **Copy output and share with me**

**Let's find out what's wrong!** ??
