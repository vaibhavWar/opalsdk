#!/bin/bash

# Test script for Product Description Generator
# Usage: ./test.sh [base_url]

# Default to localhost if no URL provided
BASE_URL="${1:-http://localhost:8787}"

echo "Testing Product Description Generator"
echo "Base URL: $BASE_URL"
echo "========================================"
echo ""

# Test 1: Health Check
echo "Test 1: Health Check (GET /)"
echo "----------------------------"
curl -s "$BASE_URL/" | jq '.'
echo ""
echo ""

# Test 2: Discovery Endpoint
echo "Test 2: Discovery Endpoint (GET /discovery)"
echo "-------------------------------------------"
curl -s "$BASE_URL/discovery" | jq '.'
echo ""
echo ""

# Test 3: Simple Product Description
echo "Test 3: Simple Product Description"
echo "-----------------------------------"
curl -s -X POST "$BASE_URL/" \
  -H "Content-Type: application/json" \
  -d '{
    "productName": "Professional Drill Set",
    "partNumber": "DRL-2024-PRO",
    "attributes": [
      "Color: Black and Orange",
      "Power: 20V Lithium-Ion",
      "Chuck Size: 1/2 inch"
    ]
  }' | jq '.'
echo ""
echo ""

# Test 4: Detailed Product Description
echo "Test 4: Detailed Product Description"
echo "-------------------------------------"
curl -s -X POST "$BASE_URL/" \
  -H "Content-Type: application/json" \
  -d '{
    "productName": "Wireless Bluetooth Keyboard",
    "partNumber": "KB-W-2024-BT",
    "attributes": [
      "Connectivity: Bluetooth 5.0",
      "Battery: Rechargeable Lithium-Ion",
      "Keys: 78-key compact layout",
      "Compatibility: Windows, Mac, iOS, Android",
      "Range: Up to 30 feet"
    ]
  }' | jq '.success'
echo ""
echo ""

# Test 5: Minimal Product (No Attributes)
echo "Test 5: Minimal Product (No Attributes)"
echo "---------------------------------------"
curl -s -X POST "$BASE_URL/" \
  -H "Content-Type: application/json" \
  -d '{
    "productName": "Standard Office Stapler",
    "partNumber": "STA-001",
    "attributes": []
  }' | jq '.success'
echo ""
echo ""

# Test 6: Error Handling - Missing Required Field
echo "Test 6: Error Handling (Missing productName)"
echo "--------------------------------------------"
curl -s -X POST "$BASE_URL/" \
  -H "Content-Type: application/json" \
  -d '{
    "partNumber": "TEST-001"
  }' | jq '.'
echo ""
echo ""

echo "========================================"
echo "All tests completed!"

