/**
 * Optimizely Opal Custom Tool: Product Description Generator
 * Implements Optimizely Opal Tools SDK Pattern
 * 
 * Generates product descriptions based on:
 * - Product Name
 * - Product Number (Part #)
 * - Product Attributes
 */

// SDK-compatible type definitions
interface ToolParameter {
  name: string;
  type: 'string' | 'number' | 'boolean' | 'array' | 'object';
  description: string;
  required: boolean;
  example?: any;
  items?: {
    type: string;
  };
}

interface ToolDefinition {
  name: string;
  description: string;
  version: string;
  parameters: ToolParameter[];
}

interface OpalTool {
  name: string;
  description: string;
  version: string;
  getDefinition(): ToolDefinition;
  execute(params: any): Promise<any>;
}

// Tool-specific interfaces
interface ProductDescriptionParams {
  productName: string;
  partNumber: string;
  attributes: string[];
}

interface ProductDescriptionResult {
  content: string;
  productName: string;
  partNumber: string;
  attributeCount: number;
}

/**
 * @OpalTool decorator pattern
 * Product Description Generator Tool
 * This class implements the Opal Tool interface following SDK patterns
 */
class ProductDescriptionGeneratorTool implements OpalTool {
  // Tool metadata (following Opal SDK pattern)
  readonly name = 'product-description-generator';
  readonly description = 'Generates concise 500-character product descriptions based on product name, part number, and attributes.';
  readonly version = '1.0.0';

  /**
   * Get tool definition for Optimizely Opal discovery
   * This method is called by Opal to understand the tool's capabilities
   */
  getDefinition(): ToolDefinition {
    return {
      name: this.name,
      description: this.description,
      version: this.version,
      parameters: [
        {
          name: 'productName',
          type: 'string',
          description: 'The name of the product',
          required: true,
          example: 'Professional Drill Set'
        },
        {
          name: 'partNumber',
          type: 'string',
          description: 'The product part number or SKU',
          required: true,
          example: 'DRL-2024-PRO'
        },
        {
          name: 'attributes',
          type: 'array',
          description: 'List of product attributes, features, or specifications (e.g., ["Color: Blue", "Power: 20V", "Material: Steel"])',
          required: true,
          example: ['Color: Blue', 'Power: 20V', 'Weight: 3.5 lbs'],
          items: {
            type: 'string'
          }
        }
      ]
    };
  }

  /**
   * Execute the tool with given parameters
   * This is the main entry point called by Optimizely Opal
   * 
   * @param params - Product description parameters
   * @returns Promise with the generated description
   */
  async execute(params: ProductDescriptionParams): Promise<ProductDescriptionResult> {
    // Validate required parameters
    if (!params.productName || !params.partNumber || !params.attributes) {
      throw new Error('Missing required parameters: productName, partNumber, and attributes are required');
    }

    // Validate attributes is an array
    if (!Array.isArray(params.attributes) || params.attributes.length === 0) {
      throw new Error('attributes must be a non-empty array');
    }

    const attributes = params.attributes;

    // Generate the product description
    const description = this.generateDescription(
      params.productName,
      params.partNumber,
      attributes
    );

    return {
      content: description,
      productName: params.productName,
      partNumber: params.partNumber,
      attributeCount: attributes.length
    };
  }

  /**
   * Generate concise 500-character product description
   * @private
   */
  private generateDescription(
    productName: string,
    partNumber: string,
    attributes: string[]
  ): string {
    // Build concise description
    let description = `${productName} (Part #: ${partNumber}) - `;
    
    // Add key selling point
    description += `A premium quality product designed for exceptional performance and reliability. `;
    
    // Add attributes if provided
    if (attributes.length > 0) {
      description += `Features include: ${attributes.slice(0, 3).join(', ')}`;
      if (attributes.length > 3) {
        description += `, and ${attributes.length - 3} more`;
      }
      description += `. `;
    }
    
    // Add closing statement
    description += `Engineered with precision and built to last, offering the perfect balance of quality, functionality, and value for professional use.`;
    
    // Trim to approximately 500 characters
    if (description.length > 500) {
      description = description.substring(0, 497) + '...';
    }
    
    return description;
  }
}

// ===================================================================
// Cloudflare Workers Integration
// This section integrates the Opal Tool with Cloudflare Workers
// ===================================================================

// Create tool instance following Opal SDK pattern
const tool = new ProductDescriptionGeneratorTool();

/**
 * Cloudflare Workers request handler
 * Implements the Optimizely Opal tool endpoints
 */
export default {
  async fetch(request: Request): Promise<Response> {
    const url = new URL(request.url);

    // CORS headers for web integration
    const corsHeaders = {
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
      'Access-Control-Allow-Headers': 'Content-Type, Authorization, X-Opal-Request-Id',
    };

    // Handle CORS preflight requests
    if (request.method === 'OPTIONS') {
      return new Response(null, { headers: corsHeaders });
    }

    try {
      // ===================================================================
      // Discovery Endpoint (GET /discovery)
      // Required by Optimizely Opal to discover tool capabilities
      // ===================================================================
      if (request.method === 'GET' && url.pathname === '/discovery') {
        const definition = tool.getDefinition();
        
        // Format response according to Opal discovery specification
        const discoveryResponse = {
          functions: [
            {
              name: definition.name,
              description: definition.description,
              version: definition.version,
              parameters: definition.parameters,
              endpoint: '/',
              http_method: 'POST',
              auth_requirements: []
            }
          ]
        };

        return new Response(JSON.stringify(discoveryResponse, null, 2), {
          headers: {
            'Content-Type': 'application/json',
            ...corsHeaders,
          },
        });
      }

      // ===================================================================
      // Health Check Endpoint (GET /)
      // Provides tool status and version information
      // ===================================================================
      if (request.method === 'GET' && url.pathname === '/') {
        return new Response(JSON.stringify({
          status: 'healthy',
          tool: tool.name,
          version: tool.version,
          description: tool.description,
          sdk_pattern: 'optimizely-opal-tools-sdk',
          endpoints: {
            discovery: '/discovery',
            health: '/',
            execute: 'POST /'
          }
        }, null, 2), {
          headers: {
            'Content-Type': 'application/json',
            ...corsHeaders,
          },
        });
      }

      // ===================================================================
      // Tool Execution Endpoint (POST /)
      // Main endpoint called by Optimizely Opal to execute the tool
      // ===================================================================
      if (request.method === 'POST' && url.pathname === '/') {
        const body = await request.json() as any;
        
        // Extract parameters from various possible Opal invocation formats
        let params: ProductDescriptionParams;
        
        if (body.parameters) {
          // Format: { parameters: { productName: "...", ... } }
          params = body.parameters;
        } else if (body.arguments) {
          // Format: { arguments: { productName: "...", ... } }
          params = body.arguments;
        } else if (body.input) {
          // Format: { input: { productName: "...", ... } }
          params = body.input;
        } else {
          // Direct format: { productName: "...", ... }
          params = body;
        }

        // Execute the tool using SDK pattern
        const result = await tool.execute(params);

        // Return response in Opal-compatible format
        return new Response(JSON.stringify({
          success: true,
          result: result,
          content: result.content,
          metadata: {
            tool: tool.name,
            version: tool.version,
            productName: result.productName,
            partNumber: result.partNumber,
            attributeCount: result.attributeCount
          }
        }, null, 2), {
          headers: {
            'Content-Type': 'application/json',
            ...corsHeaders,
          },
        });
      }

      // Route not found
      return new Response(JSON.stringify({
        error: 'Not Found',
        message: 'Available endpoints: GET /discovery, GET / (health), POST / (execute)',
        tool: tool.name,
        version: tool.version,
        endpoints: {
          discovery: '/discovery',
          health: '/',
          execute: 'POST /'
        }
      }), {
        status: 404,
        headers: {
          'Content-Type': 'application/json',
          ...corsHeaders,
        },
      });

    } catch (error) {
      // Error handling with detailed information
      return new Response(JSON.stringify({
        success: false,
        error: error instanceof Error ? error.message : 'Unknown error',
        details: error instanceof Error ? error.stack : undefined,
        tool: tool.name,
        version: tool.version
      }), {
        status: 400,
        headers: {
          'Content-Type': 'application/json',
          ...corsHeaders,
        },
      });
    }
  },
};
