/**
 * Optimizely Opal Custom Tool: Product Description Generator
 * TypeScript Implementation for Cloudflare Workers
 * Following SDK patterns from: https://academy.optimizely.com/student/path/2839076/activity/4331694
 * 
 * This tool generates natural, AI-like product descriptions
 * based on product name, part number, and attributes.
 */

// ===================================================================
// Type Definitions (Following Opal SDK Pattern)
// ===================================================================

enum ParameterType {
  String = 'string',
  Number = 'number',
  Boolean = 'boolean',
  Array = 'array',
  Object = 'object'
}

interface ToolParameter {
  name: string;
  type: ParameterType;
  description: string;
  required: boolean;
  enum?: string[];
  items?: { type: ParameterType };
  example?: any;
}

interface ToolDefinition {
  name: string;
  description: string;
  parameters: ToolParameter[];
}

interface ProductDescriptionParams {
  productName: string;
  partNumber: string;
  attributes: string[];
  type?: string;
  tone?: string;
}

// ===================================================================
// Product Description Generator Tool (Following SDK Pattern)
// Mimics @tool decorator behavior for Cloudflare Workers
// ===================================================================

class ProductDescriptionGeneratorTool {
  // Tool metadata (similar to @tool decorator)
  readonly toolDefinition: ToolDefinition = {
    name: 'product-description-generator',
    description: 'Generates natural, AI-like product descriptions dynamically based on any product attributes.',
    parameters: [
      {
        name: 'productName',
        type: ParameterType.String,
        description: 'The name of the product',
        required: true,
        example: 'DEWALT 20V Acrylic Dispenser 101 28 oz'
      },
      {
        name: 'partNumber',
        type: ParameterType.String,
        description: 'The product part number or SKU',
        required: true,
        example: '211DCE595D1'
      },
      {
        name: 'attributes',
        type: ParameterType.Array,
        description: 'List of product attributes, features, or specifications (e.g., ["Brand: DEWALT", "Voltage: 20V"])',
        required: true,
        items: { type: ParameterType.String },
        example: ['Brand: DEWALT', 'Battery Voltage (V): 20', 'Capacity: 28 oz.']
      },
      {
        name: 'type',
        type: ParameterType.String,
        description: 'The type or category of content (e.g., "ecommerce", "technical", "marketing"). Defaults to "general".',
        required: false,
        enum: ['ecommerce', 'technical', 'marketing', 'general'],
        example: 'ecommerce'
      },
      {
        name: 'tone',
        type: ParameterType.String,
        description: 'The tone of the description (e.g., "professional", "casual", "enthusiastic"). Defaults to "professional".',
        required: false,
        enum: ['professional', 'casual', 'enthusiastic'],
        example: 'professional'
      }
    ]
  };

  /**
   * Execute method - main entry point (following SDK pattern)
   * Similar to async execute() in the Optimizely Academy examples
   */
  async execute(params: ProductDescriptionParams) {
    // Validate required parameters
    if (!params.productName || !params.partNumber || !params.attributes) {
      throw new Error('Missing required parameters: productName, partNumber, and attributes are required');
    }

    // Validate attributes is an array
    if (!Array.isArray(params.attributes) || params.attributes.length === 0) {
      throw new Error('attributes must be a non-empty array');
    }

    const type = params.type || 'general';
    const tone = params.tone || 'professional';

    // Generate the product description
    const description = this.generateDescription(
      params.productName,
      params.partNumber,
      params.attributes,
      type,
      tone
    );

    return {
      content: description,
      productName: params.productName,
      partNumber: params.partNumber,
      attributeCount: params.attributes.length,
      type: type,
      tone: tone
    };
  }

  /**
   * Generate natural, AI-like product description dynamically
   * @private
   */
  private generateDescription(
    productName: string,
    partNumber: string,
    attributes: string[],
    type: string,
    tone: string
  ): string {
    // Parse attributes into a map for easier access
    const attrMap = new Map<string, string>();
    const attrList: Array<{ key: string; value: string; original: string }> = [];
    
    attributes.forEach(attr => {
      const colonIndex = attr.indexOf(':');
      if (colonIndex > 0) {
        const key = attr.substring(0, colonIndex).trim().toLowerCase();
        const value = attr.substring(colonIndex + 1).trim();
        attrMap.set(key, value);
        attrList.push({ key, value, original: attr });
      } else {
        attrList.push({ key: attr.toLowerCase(), value: attr, original: attr });
      }
    });

    // Start with engaging introduction
    let description = `The ${productName} (Part# ${partNumber}) `;
    
    // Add opening statement - check for power/voltage/cordless to make it dynamic
    const hasPower = attrMap.has('battery voltage (v)') || attrMap.has('voltage') || attrMap.has('power');
    const isCordless = attrMap.get('cordless / corded')?.toLowerCase() === 'cordless';
    
    // Adjust opening based on type and tone
    if (type.toLowerCase() === 'ecommerce' && tone.toLowerCase() === 'professional') {
      if (hasPower && isCordless) {
        description += 'delivers powerful, reliable cordless performance for professional applications. ';
      } else if (hasPower) {
        description += 'offers consistent, professional-grade powered performance. ';
      } else {
        description += 'provides exceptional professional-grade quality and reliability. ';
      }
    } else {
      // Default behavior
      if (hasPower && isCordless) {
        description += 'delivers powerful cordless performance. ';
      } else if (hasPower) {
        description += 'offers reliable powered performance. ';
      } else {
        description += 'provides professional-grade quality. ';
      }
    }
    
    // Build feature highlights from meaningful attributes with context
    const priorityKeys = ['capacity', 'cartridge type', 'weight', 'dimensions', 'material'];
    const meaningfulAttrs = attrList
      .filter(a => 
        !a.key.includes('cs_') && 
        a.key !== 'brand' &&
        a.key !== 'cordless / corded' &&
        a.value.toLowerCase() !== 'yes' && 
        a.value.toLowerCase() !== 'no' &&
        a.value.length > 2
      )
      .sort((a, b) => {
        const aIsPriority = priorityKeys.some(k => a.key.includes(k));
        const bIsPriority = priorityKeys.some(k => b.key.includes(k));
        if (aIsPriority && !bIsPriority) return -1;
        if (!aIsPriority && bIsPriority) return 1;
        return 0;
      })
      .slice(0, 3);
    
    if (meaningfulAttrs.length > 0) {
      const features = meaningfulAttrs.map(a => {
        // For better readability, include key for context if value doesn't already contain it
        if (a.value.length < 15 && !a.value.toLowerCase().includes(a.key.split(' ')[0])) {
          return `${a.key}: ${a.value}`;
        }
        return a.value;
      }).join(', ');
      description += `Features include ${features}. `;
    }
    
    // Add brand statement if available
    const brand = attrMap.get('brand');
    if (brand) {
      description += `Built with ${brand} quality and reliability. `;
    }
    
    // Add warranty if available
    const warranty = attrMap.get('cs_manufacturer_warranty');
    if (warranty) {
      const warrantySimple = warranty.replace(' limited warranty', '').split('/')[0];
      description += `Backed by ${warrantySimple} warranty. `;
    } else {
      description += 'Designed for demanding applications. ';
    }
    
    return description.trim();
  }
}

// ===================================================================
// Tool Service (Following SDK Pattern)
// Manages tool registration and execution
// ===================================================================

class ToolsService {
  private tools: Map<string, ProductDescriptionGeneratorTool> = new Map();

  registerTool(tool: ProductDescriptionGeneratorTool) {
    this.tools.set(tool.toolDefinition.name, tool);
  }

  getDiscovery() {
    const functions = Array.from(this.tools.values()).map(tool => ({
      name: tool.toolDefinition.name,
      description: tool.toolDefinition.description,
      parameters: tool.toolDefinition.parameters,
      endpoint: '/',
      http_method: 'POST',
      auth_requirements: []
    }));

    return { functions };
  }

  async executeTool(toolName: string, params: any) {
    const tool = this.tools.get(toolName);
    if (!tool) {
      throw new Error(`Tool '${toolName}' not found`);
    }
    return await tool.execute(params);
  }
}

// ===================================================================
// Initialize Service and Register Tool
// ===================================================================

const toolsService = new ToolsService();
const productDescriptionTool = new ProductDescriptionGeneratorTool();
toolsService.registerTool(productDescriptionTool);

// ===================================================================
// Cloudflare Workers Fetch Handler
// Routes requests through the Tools Service
// ===================================================================

export default {
  async fetch(request: Request, env: any, ctx: any): Promise<Response> {
    const url = new URL(request.url);
    
    try {
      // Route discovery endpoint
      if (url.pathname === '/discovery' && request.method === 'GET') {
        const discovery = toolsService.getDiscovery();
        return new Response(JSON.stringify(discovery), {
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        });
      }
      
      // Route tool execution
      if (url.pathname === '/' && request.method === 'POST') {
        const body = await request.json() as any;
        const result = await toolsService.executeTool('product-description-generator', body);
        return new Response(JSON.stringify(result), {
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        });
      }
      
      // Health check
      if (url.pathname === '/' && request.method === 'GET') {
        return new Response(JSON.stringify({ 
          status: 'healthy',
          service: 'Product Description Generator',
          version: '1.0.0'
        }), {
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        });
      }
      
      // Handle CORS preflight
      if (request.method === 'OPTIONS') {
        return new Response(null, {
          headers: {
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
            'Access-Control-Allow-Headers': 'Content-Type'
          }
        });
      }
      
      // Not found
      return new Response('Not Found', { status: 404 });
      
    } catch (error: any) {
      return new Response(JSON.stringify({ 
        error: error.message || 'Internal Server Error'
      }), {
        status: 500,
        headers: {
          'Content-Type': 'application/json',
          'Access-Control-Allow-Origin': '*'
        }
      });
    }
  }
};
