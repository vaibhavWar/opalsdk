# Troubleshooting Guide - Product Description Generator

## Common Issues & Solutions

### Issue: "Sorry. There was an unanticipated error while processing that request."

This error from Opal usually means one of the following:

#### 1. Tool Not Accessible
**Problem**: Opal cannot reach your tool endpoint

**Solution**:
- Ensure your C# app is running: `dotnet run --project csharp/ProductDescriptionGenerator.csproj`
- Check the app is listening on the correct port (default: 5000)
- If Opal is on a different machine, ensure firewall allows incoming connections
- Use a public URL (ngrok, Azure, etc.) instead of localhost

#### 2. Discovery Endpoint Issues
**Problem**: Opal cannot discover the tool properly

**Test Discovery**:
```bash
curl http://localhost:5000/discovery
```

**Expected Response**: JSON with tool definition
```json
{
  "functions": [{
    "name": "product-description-generator",
    "description": "Generates natural...",
    "parameters": [...],
    "endpoint": "/tools/product-description-generator",
    "http_method": "POST"
  }]
}
```

#### 3. CORS Issues
**Problem**: Browser blocks the request

**Check CORS**:
- Open browser developer tools (F12)
- Look for CORS errors in the Console tab
- Check Network tab for OPTIONS requests

**Solution**: The app already has CORS enabled (`AllowAnyOrigin`), but ensure no middleware is blocking it.

#### 4. Request Format Issues
**Problem**: Opal sends request in unexpected format

**Correct Request Format**:
```json
{
  "parameters": {
    "ProductName": "DEWALT 20V Tool",
    "PartNumber": "12345",
    "Attributes": ["Brand: DEWALT", "Voltage: 20V"]
  }
}
```

**Note**: The parameters MUST be wrapped in a `parameters` object for the Opal SDK.

#### 5. Missing Required Fields
**Problem**: Required parameters not provided

**Required Parameters**:
- `ProductName` (string) - Required
- `PartNumber` (string) - Required
- `Attributes` (array) - Required, must not be empty

**Optional Parameters**:
- `Type` (string) - Defaults to "general"
- `Tone` (string) - Defaults to "professional"

---

## Diagnostic Steps

### Step 1: Check if App is Running
```powershell
# PowerShell
$response = Invoke-RestMethod -Uri "http://localhost:5000/discovery" -Method Get
$response | ConvertTo-Json -Depth 10
```

### Step 2: Test Tool Manually
```powershell
# PowerShell
$body = @{
    parameters = @{
        ProductName = "Test Product"
        PartNumber = "TEST-001"
        Attributes = @("Brand: Test", "Color: Blue")
    }
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/tools/product-description-generator" `
  -Method Post `
  -Body $body `
  -ContentType "application/json"
```

### Step 3: Check Logs
The app now has enhanced logging. Check the console output for:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: ProductDescriptionGenerator.Program[0]
      Product Description Generator started
      Discovery endpoint: /discovery
      Tool endpoint: /tools/product-description-generator
```

If you see errors, they will show with:
```
error: ProductDescriptionGenerator[0]
      Unhandled exception occurred
      System.Exception: [error details]
```

### Step 4: Test from External Machine
If Opal is on a different machine:

```bash
# From the Opal machine
curl http://YOUR_MACHINE_IP:5000/discovery
```

If this fails, you need to:
1. Allow port 5000 through Windows Firewall
2. Configure app to listen on all interfaces:
   ```bash
   dotnet run --urls "http://0.0.0.0:5000"
   ```

---

## Using ngrok for Testing

If Opal cannot reach your localhost:

### Install ngrok
Download from: https://ngrok.com/

### Expose Your Local App
```bash
ngrok http 5000
```

This gives you a public URL like: `https://abc123.ngrok.io`

### Update Opal Configuration
Use the ngrok URL in Opal:
- Discovery: `https://abc123.ngrok.io/discovery`
- Tool URL will be auto-detected

---

## Common Error Messages

### "ProductName is required"
**Cause**: Missing ProductName in request  
**Fix**: Ensure request includes `ProductName` in parameters

### "attributes must be a non-empty array"
**Cause**: Attributes array is empty or null  
**Fix**: Provide at least one attribute: `["Brand: XYZ"]`

### "The remote server returned an error: (500)"
**Cause**: Server-side error in tool execution  
**Fix**: Check app logs for exception details

### "Unable to connect to the remote server"
**Cause**: App not running or firewall blocking  
**Fix**: Ensure app is running and accessible

---

## Enable Detailed Logging

To see more detailed logs, run:

```bash
cd csharp
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

This enables:
- Detailed exception pages
- More verbose logging
- Stack traces in responses

---

## Testing with cURL

### Test Discovery
```bash
curl -v http://localhost:5000/discovery
```

### Test Execution
```bash
curl -v -X POST http://localhost:5000/tools/product-description-generator \
  -H "Content-Type: application/json" \
  -d '{
    "parameters": {
      "ProductName": "DEWALT 20V Tool",
      "PartNumber": "12345",
      "Attributes": ["Brand: DEWALT", "Voltage: 20V"],
      "Type": "ecommerce",
      "Tone": "professional"
    }
  }'
```

---

## Still Having Issues?

1. **Restart the C# app** - Stop and restart the application
2. **Check Opal logs** - Look in Opal's logs for more details
3. **Verify network** - Ensure Opal can reach your machine
4. **Try ngrok** - Use ngrok to eliminate networking issues
5. **Check Opal configuration** - Verify the tool URL in Opal matches your endpoint

---

## Getting Help

If you're still stuck, gather this information:

1. Output of `curl http://localhost:5000/discovery`
2. Console logs from the C# app
3. Error message from Opal (full text)
4. Network tab from browser (if using Opal web UI)
5. Request payload being sent to the tool

This will help diagnose the issue faster.

