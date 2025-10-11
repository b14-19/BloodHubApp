# ?? Azure Deployment Guide - BloodHub Notifications

## ? What Was Implemented

### 1. **Dynamic Donor Notifications**
- ? No hardcoded emails or phone numbers
- ? Automatically fetches matching blood group donors from database
- ? Only notifies donors with `ServiceType = "Free"`
- ? Sends emails to all matching donors when a blood request is created

### 2. **Features**
- ?? **Email Notifications**: Sent to all donors with matching blood type
- ?? **Smart Matching**: Only donors who opted for "Free" service
- ?? **Fail-Safe**: If email fails, request still succeeds
- ?? **Logging**: All notification activities logged for debugging

### 3. **Database Changes**
- Added `BloodGroup` column to `Donors` table
- Migration created: `AddBloodGroupToDonor`

---

## ?? Azure Configuration Steps

### **Step 1: Configure SMTP Settings in Azure**

1. Go to **Azure Portal** ? Your App Service
2. Click **Configuration** (under Settings)
3. Click **+ New application setting**
4. Add these settings one by one:

| Name | Value | Example |
|------|-------|---------|
| `Notifications__Smtp__Host` | `smtp.gmail.com` | Gmail SMTP server |
| `Notifications__Smtp__Port` | `587` | Standard SMTP port |
| `Notifications__Smtp__From` | `your-email@gmail.com` | Your sender email |
| `Notifications__Smtp__User` | `your-email@gmail.com` | Your Gmail username |
| `Notifications__Smtp__Pass` | `your-app-password` | Gmail App Password (NOT regular password) |

5. Click **Save**
6. Click **Restart** to apply changes

?? **Important**: Use double underscore `__` in setting names for nested configuration!

---

### **Step 2: Generate Gmail App Password**

1. Go to [Google Account Security](https://myaccount.google.com/security)
2. Enable **2-Step Verification** (if not already enabled)
3. Search for "App passwords" and click it
4. Select **Mail** and **Other (Custom name)** ? Type "BloodHub Azure"
5. Click **Generate**
6. Copy the 16-character password (no spaces)
7. Use this in `Notifications__Smtp__Pass` setting

---

### **Step 3: Deploy Application**

#### Option A: GitHub Actions (Recommended)
```yaml
# Already set up in your repository
git push origin master
```

#### Option B: Visual Studio Publish
1. Right-click project ? **Publish**
2. Select your Azure App Service
3. Click **Publish**

#### Option C: Azure CLI
```bash
cd BloodHubApp
dotnet publish -c Release
cd bin/Release/net9.0/publish
az webapp deployment source config-zip --resource-group <resource-group> --name <app-name> --src <zip-file>
```

---

### **Step 4: Update Database Schema**

After deployment, run migrations:

1. Go to **Azure Portal** ? Your App Service ? **Console** (under Development Tools)
2. Run:
```bash
cd site/wwwroot
dotnet BloodHubApp.dll
```

Or use **Connection strings** to run migrations from local:
```bash
dotnet ef database update --connection "your-azure-connection-string"
```

---

## ?? Testing Notifications

### **Test Scenario 1: Create Donor with Blood Group**

1. Login to application
2. Go to **Donor Service** page
3. Fill in donor details:
   - Name: Test Donor
   - Email: test@example.com
   - Blood Group: **A+** (or any blood type)
   - Service Type: **Free**
   - Hospital: Select any
4. Click **Submit**

### **Test Scenario 2: Create Blood Request**

1. Go to **Requests** page
2. Click **+ New Request**
3. Fill in:
   - Hospital: Select any
   - Blood Group: **A+** (match the donor's blood group)
   - Units: 2
   - Patient details: Fill all fields
4. Click **Save Request**

### **Expected Result:**
- ? Request created successfully
- ? Email sent to test@example.com
- ? Check Azure logs to verify email was sent

---

## ?? Monitoring & Troubleshooting

### **View Logs in Azure**

1. Go to **Azure Portal** ? Your App Service
2. Click **Log stream** (under Monitoring)
3. Look for log entries:
   - `Blood request created: A+ at Hospital Name. Found 3 matching donors`
   - `Notification sent to donor: donor@example.com`
   - `Email sent to {email}` (from NotificationService)

### **Common Issues**

| Issue | Solution |
|-------|----------|
| No emails sent | Check if `Notifications__Smtp__*` settings are configured |
| Email fails | Verify Gmail App Password is correct (no spaces) |
| "SMTP not configured" | Double-check setting names use `__` (double underscore) |
| No matching donors | Ensure donors exist with matching blood group and `ServiceType = "Free"` |

### **Enable Detailed Logging**

Add this to Azure App Service Configuration:

| Name | Value |
|------|-------|
| `Logging__LogLevel__BloodHub` | `Debug` |
| `Logging__LogLevel__BloodHubApp.Services` | `Information` |

---

## ?? How It Works

### **Notification Flow:**

```
1. Hospital creates blood request (A+ blood needed)
   ?
2. System queries database:
   SELECT * FROM Donors 
   WHERE BloodGroup = 'A+' 
   AND ServiceType = 'Free' 
   AND Email IS NOT NULL
   ?
3. For each matching donor:
   - Build email with request details
   - Send email via SMTP
   - Log success/failure
   ?
4. Return success message to user
```

### **Email Template:**

```
Subject: ?? Urgent: A+ Blood Needed at City Hospital

Body:
??????????????????????????????????????
?   Urgent Blood Request            ?
??????????????????????????????????????

Hospital: City Hospital, Mumbai
Blood Group Needed: A+
Units Required: 2
Patient Name: John Doe
Contact: +91 9876543210
Address: 123 Main St
Requested On: 15 Jan 2025, 02:30 PM

?????????????????????????????????????
Thank you for being a registered blood donor.
If you can donate, please contact the hospital immediately.

This is an automated notification from BloodHub.
```

---

## ?? Summary

### What Works:
- ? Email notifications to matching donors
- ? Dynamic data from database
- ? No hardcoded values
- ? Works on Azure
- ? Fail-safe (doesn't break if email fails)

### What's NOT Implemented:
- ? SMS notifications (not free in India, requires paid API)
- ? WhatsApp notifications (requires Business API)

### Configuration Required:
1. Gmail App Password
2. Azure App Service Configuration settings
3. Database migration (already created)

---

## ?? Support

If you encounter issues:

1. **Check Logs**: Azure Portal ? Log Stream
2. **Verify Settings**: Configuration ? Application Settings
3. **Test SMTP**: Use the NotificationService directly
4. **Check Database**: Verify donors exist with correct blood groups

---

## ?? You're Done!

Your BloodHub application now automatically notifies matching blood donors when a request is created!

**Next Steps:**
1. Deploy to Azure
2. Configure SMTP settings
3. Add some test donors
4. Create a blood request
5. Check your email! ??
