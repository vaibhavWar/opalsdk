# Optimizely Opal Tool URL Structure

## Correct URL Pattern

All Optimizely Opal custom tools must use the `/tools/` prefix in their URL structure.

---

## Endpoint URLs

### **1. Discovery Endpoint**
```
GET /tools/discovery
```

**Purpose:** Allows Optimizely Opal to discover the tool's capabilities, parameters, and metadata.

**Full URL:** `https://your-worker.workers.dev/tools/discovery`

**Response:**
```json
{
  "functions": [
    {
      "name": "product-description-generator",
      "description": "...",
      "version": "1.0.0",
      "parameters": [...],
      "endpoint": "/tools/execute",
      "http_method": "POST"
    }
  ]
}
```

---

### **2. Health Check Endpoint**
```
GET /tools/health
GET /tools
GET /
```

**Purpose:** Provides tool status and version information for monitoring.

**Full URL:** `https://your-worker.workers.dev/tools/health`

**Response:**
```json
{
  "status": "healthy",
  "tool": "product-description-generator",
  "version": "1.0.0",
  "sdk_pattern": "optimizely-opal-tools-sdk",
  "endpoints": {
    "discovery": "/tools/discovery",
    "health": "/tools/health",
    "execute": "POST /tools/execute"
  }
}
```

---

### **3. Execute Endpoint**
```
POST /tools/execute
```

**Purpose:** Main endpoint called by Optimizely Opal to execute the tool.

**Full URL:** `https://your-worker.workers.dev/tools/execute`

**Request:**
```json
{
  "productName": "Smart Watch Pro",
  "partNumber": "SW-2024",
  "attributes": ["AMOLED Display", "GPS"]
}
```

**Response:**
```json
{
  "success": true,
  "result": {
    "content": "# Product Description...",
    "productName": "Smart Watch Pro",
    "partNumber": "SW-2024",
    "attributeCount": 2
  },
  "metadata": {
    "tool": "product-description-generator",
    "version": "1.0.0",
    ...
  }
}
```

---

## Why `/tools/` Prefix?

### **Optimizely Requirements**
- ✅ Standard convention for Opal custom tools
- ✅ Namespace separation from other endpoints
- ✅ Easier routing and management
- ✅ Consistent with Opal platform expectations

### **Benefits**
- **Clarity:** Immediately identifies tool-related endpoints
- **Organization:** Groups all tool endpoints under one path
- **Scalability:** Easy to add more tools under `/tools/`
- **Standards:** Follows Optimizely Opal conventions

---

## Testing Locally

### Test Discovery
```bash
curl http://localhost:8787/tools/discovery
```

### Test Health
```bash
curl http://localhost:8787/tools/health
```

### Test Execution
```bash
curl -X POST http://localhost:8787/tools/execute \
  -H "Content-Type: application/json" \
  -d '{
    "productName": "Test Product",
    "partNumber": "TP-001",
    "attributes": ["Feature 1", "Feature 2"]
  }'
```

### PowerShell Testing
```powershell
# Discovery
Invoke-RestMethod -Uri "http://localhost:8787/tools/discovery"

# Health
Invoke-RestMethod -Uri "http://localhost:8787/tools/health"

# Execute
$body = @{
    productName = "Test Product"
    partNumber = "TP-001"
    attributes = @("Feature 1", "Feature 2")
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:8787/tools/execute" `
  -Method Post -Body $body -ContentType "application/json"
```

---

## Registering with Optimizely Opal

When registering your custom tool with Opal:

### **Step 1:** Deploy to Cloudflare Workers
```bash
npm run deploy
```

You'll get a URL like:
```
https://product-description-generator.your-name.workers.dev
```

### **Step 2:** Register with Opal
1. Go to: **https://opal.optimizely.com/tools**
2. Click **"Add Custom Tool"**
3. Enter your discovery URL:
   ```
   https://product-description-generator.your-name.workers.dev/tools/discovery
   ```
4. Click **"Save"**

### **Step 3:** Opal Auto-Configuration
Opal will automatically:
- Call `/tools/discovery` to learn about your tool
- Parse parameter definitions
- Configure the execute endpoint (`/tools/execute`)
- Make the tool available in Opal chat

---

## URL Structure Comparison

### ❌ **Incorrect (Old Pattern)**
```
GET  /discovery          ❌
GET  /                   ❌
POST /                   ❌
```

### ✅ **Correct (Optimizely Pattern)**
```
GET  /tools/discovery    ✅
GET  /tools/health       ✅
POST /tools/execute      ✅
```

---

## Implementation in Code

```typescript
export default {
  async fetch(request: Request): Promise<Response> {
    const url = new URL(request.url);
    
    // Discovery endpoint
    if (request.method === 'GET' && url.pathname === '/tools/discovery') {
      // Return tool definition
    }
    
    // Health check
    if (request.method === 'GET' && url.pathname === '/tools/health') {
      // Return health status
    }
    
    // Execute endpoint
    if (request.method === 'POST' && url.pathname === '/tools/execute') {
      // Execute tool logic
    }
  }
};
```

---

## Multiple Tool Support (Future)

The `/tools/` prefix allows for multiple tools on the same worker:

```
GET  /tools/discovery                     # Lists all tools
GET  /tools/product-description/execute   # Tool 1
GET  /tools/seo-optimizer/execute         # Tool 2
GET  /tools/image-analyzer/execute        # Tool 3
```

---

## Error Responses

### 404 Not Found
When accessing an invalid endpoint:

```json
{
  "error": "Not Found",
  "message": "Available endpoints: GET /tools/discovery, GET /tools/health, POST /tools/execute",
  "endpoints": {
    "discovery": "/tools/discovery",
    "health": "/tools/health",
    "execute": "POST /tools/execute"
  }
}
```

---

## CORS Configuration

All `/tools/*` endpoints include CORS headers:

```typescript
const corsHeaders = {
  'Access-Control-Allow-Origin': '*',
  'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
  'Access-Control-Allow-Headers': 'Content-Type, Authorization, X-Opal-Request-Id',
};
```

---

## Summary

### ✅ **Implemented URL Structure**

| Endpoint | Method | Path | Purpose |
|----------|--------|------|---------|
| **Discovery** | GET | `/tools/discovery` | Tool capabilities |
| **Health** | GET | `/tools/health` | Status check |
| **Execute** | POST | `/tools/execute` | Tool execution |

### ✅ **Verification**

All endpoints tested and working:
- ✅ `/tools/discovery` returns correct tool definition
- ✅ `/tools/health` returns status with all endpoint URLs
- ✅ `/tools/execute` executes tool and returns results
- ✅ All responses include proper SDK metadata
- ✅ CORS headers configured correctly

### ✅ **Opal Registration URL**

```
https://your-worker.workers.dev/tools/discovery
```

---

**The tool now follows the correct Optimizely Opal URL structure!** ✅

