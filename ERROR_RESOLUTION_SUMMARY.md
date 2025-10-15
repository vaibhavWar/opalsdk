# Error Resolution Summary

## Your Issue
You received this error from Optimizely Opal:
> "Sorry. There was an unanticipated error while processing that request."

## What I Fixed

### 1. ✅ Added Comprehensive Error Handling

**Program.cs Updates:**
- Global error handling middleware catches all unhandled exceptions
- Enhanced logging (Console + Debug)
- Startup logs show endpoints and status
- Errors are logged with full details

**Tool Updates:**
- Wrapped execution in try-catch blocks
- Returns structured error responses
- Validation errors separated from processing errors
- Success flag added to all responses

### 2. ✅ Improved Error Messages

**Before:**
```
Generic 500 error with no details
```

**After:**
```json
{
  "error": "Validation Error",
  "message": "ProductName is required",
  "success": false
}
```

### 3. ✅ Added Troubleshooting Guide

Created **TROUBLESHOOTING.md** with:
- Common issues and solutions
- Step-by-step diagnostics
- Network configuration help
- ngrok setup for external access
- Complete testing procedures

## Testing Results ✅

I tested the updated app and confirmed:

### ✅ Discovery Works
```bash
curl http://localhost:5000/discovery
```
Returns proper tool definition

### ✅ Execution Works
```bash
curl -X POST http://localhost:5000/tools/product-description-generator \
  -d '{"parameters": {...}}'
```
Generates descriptions successfully

### ✅ Error Handling Works
Invalid requests return clear error messages instead of crashing

## Most Likely Causes of Your Error

### 1. 🔴 Network/Accessibility Issues

**Problem**: Opal cannot reach your C# app

**Check**:
- Is your app running? Look for: `Now listening on: http://localhost:5000`
- Can Opal reach your machine?
- Is there a firewall blocking port 5000?

**Solution**:
```bash
# Run app on all interfaces
dotnet run --project csharp/ProductDescriptionGenerator.csproj --urls "http://0.0.0.0:5000"

# Allow through firewall (Windows)
netsh advfirewall firewall add rule name="OpalTool" dir=in action=allow protocol=TCP localport=5000
```

### 2. 🔴 Using Localhost with Remote Opal

**Problem**: Opal is on a different machine than your C# app

**Solution - Option A: Use ngrok**
```bash
# Install ngrok from https://ngrok.com/
ngrok http 5000

# Use the provided URL in Opal (e.g., https://abc123.ngrok.io)
```

**Solution - Option B: Deploy to Azure/AWS**
Deploy your C# app to a cloud server with a public IP

### 3. 🔴 Wrong Request Format

**Problem**: Opal sends request without `parameters` wrapper

**Correct Format**:
```json
{
  "parameters": {
    "ProductName": "DEWALT Tool",
    "PartNumber": "12345",
    "Attributes": ["Brand: DEWALT"]
  }
}
```

**Opal SDK automatically wraps parameters**, so this should work correctly.

### 4. 🔴 CORS Issues

**Problem**: Browser blocks the request

**Check**: Look in browser console (F12) for CORS errors

**Solution**: Already fixed - app has `AllowAnyOrigin()` configured

## Diagnostic Steps You Should Take

### Step 1: Verify App is Running
```powershell
# PowerShell
Invoke-RestMethod -Uri "http://localhost:5000/discovery" -Method Get | ConvertTo-Json
```

**Expected**: JSON with tool definition  
**If this fails**: App isn't running or port is wrong

### Step 2: Test Tool Manually
```powershell
$body = @{
    parameters = @{
        ProductName = "Test Product"
        PartNumber = "TEST-001"
        Attributes = @("Brand: Test")
    }
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/tools/product-description-generator" `
  -Method Post -Body $body -ContentType "application/json"
```

**Expected**: Generated description  
**If this works**: The app is fine, it's a networking/Opal config issue

### Step 3: Check if Opal Can Reach You

From the machine running Opal:
```bash
curl http://YOUR_IP:5000/discovery
```

Replace `YOUR_IP` with your machine's IP address.

**If this fails**: 
- Firewall is blocking
- App is only listening on localhost
- Wrong IP/port

### Step 4: Check Opal Configuration

In your Opal tool configuration:
- **Discovery URL**: Should be `http://YOUR_IP:5000/discovery` or `https://your-ngrok.ngrok.io/discovery`
- **NOT**: `http://localhost:5000/discovery` (unless Opal is on same machine)

### Step 5: Look at App Logs

The app now logs everything. Check console output for:
```
info: ProductDescriptionGenerator.Program[0]
      Product Description Generator started
      Discovery endpoint: /discovery
      Tool endpoint: /tools/product-description-generator
```

If you see errors like:
```
error: ProductDescriptionGenerator[0]
      Unhandled exception occurred
```

That tells you what's actually failing.

## Quick Fixes to Try Now

### Fix 1: Restart with Network Binding
```bash
cd csharp
dotnet run --urls "http://0.0.0.0:5000"
```

This makes the app accessible from other machines.

### Fix 2: Use ngrok (Easiest for Testing)
```bash
# In one terminal
cd csharp
dotnet run

# In another terminal
ngrok http 5000
```

Then use the ngrok URL (e.g., `https://abc123.ngrok.io`) in Opal.

### Fix 3: Check Firewall
```powershell
# Windows PowerShell (as Administrator)
New-NetFirewallRule -DisplayName "Opal Tool" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
```

## What to Send Me if Still Broken

If you're still getting the error, send me:

1. **Output of this command**:
   ```bash
   curl http://localhost:5000/discovery
   ```

2. **Console output from the C# app** (especially any errors)

3. **Exact error message from Opal** (screenshot if possible)

4. **Are you running Opal and the C# app on the same machine?**

5. **Output of**:
   ```bash
   curl -X POST http://localhost:5000/tools/product-description-generator \
     -H "Content-Type: application/json" \
     -d '{"parameters":{"ProductName":"Test","PartNumber":"001","Attributes":["Brand: Test"]}}'
   ```

## Summary

✅ **What I Fixed:**
- Added comprehensive error handling
- Improved logging
- Created troubleshooting guide
- App now returns clear error messages

🔍 **Most Likely Issue:**
- Opal cannot reach your C# app (networking/firewall)
- Use ngrok or deploy to cloud for remote access

📝 **Next Steps:**
1. Try the diagnostic steps above
2. Check if Opal can reach your app
3. Use ngrok if Opal is on a different machine
4. Look at the app logs for specific errors

**Full troubleshooting guide**: See `TROUBLESHOOTING.md`

