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
  attributes?: string[];
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
  readonly description = 'Generates comprehensive, marketing-ready product descriptions based on product name, part number, and attributes. Creates structured content with overview, features, specifications, and applications.';
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
          required: false,
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
    if (!params.productName || !params.partNumber) {
      throw new Error('Missing required parameters: productName and partNumber are required');
    }

    // Default attributes to empty array if not provided
    const attributes = params.attributes || [];

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
   * Generate comprehensive product description with markdown formatting
   * @private
   */
  private generateDescription(
    productName: string,
    partNumber: string,
    attributes: string[]
  ): string {
    const sections: string[] = [];

    // Header
    sections.push('# Product Description');
    sections.push('');
    sections.push(`## ${productName}`);
    sections.push('');
    sections.push(`**Part Number:** \`${partNumber}\`\n`);
    sections.push('---\n');

    // Overview
    sections.push('## Overview\n');
    sections.push(this.generateOverview(productName, attributes));
    sections.push('');

    // Key Features
    sections.push('## Key Features\n');
    sections.push(this.generateKeyFeatures(productName, attributes));
    sections.push('');

    // Technical Specifications
    sections.push('## Technical Specifications\n');
    sections.push(`**Product Name:** ${productName}  `);
    sections.push(`**Part Number:** ${partNumber}\n`);

    // Product Attributes
    sections.push('## Product Attributes\n');
    if (attributes.length > 0) {
      attributes.forEach(attr => sections.push(`- ${attr}`));
    } else {
      sections.push('- No additional attributes specified');
    }
    sections.push('\n---\n');

    // Benefits
    sections.push('## Why Choose This Product?\n');
    sections.push(this.generateBenefits(productName, attributes));
    sections.push('');

    // Applications
    sections.push('## Product Applications\n');
    sections.push(this.generateApplications(attributes));
    sections.push('\n---\n');

    // Footer
    sections.push(`*Generated product description for ${productName} (Part #: ${partNumber})*`);

    return sections.join('\n');
  }

  /**
   * Generate product overview section
   * @private
   */
  private generateOverview(productName: string, attributes: string[]): string {
    let overview = `The **${productName}** is a premium product designed to deliver exceptional performance and reliability. `;
    
    if (attributes.length > 0) {
      overview += `This product features ${attributes.length} key attribute${attributes.length > 1 ? 's' : ''} that make it stand out in its category. `;
    }
    
    overview += `Engineered with precision and built to last, the ${productName} represents the perfect balance of quality, functionality, and value.`;
    
    return overview;
  }

  /**
   * Generate key features section
   * @private
   */
  private generateKeyFeatures(productName: string, attributes: string[]): string {
    const features: string[] = [];
    
    if (attributes.length > 0) {
      attributes.forEach((attr, index) => {
        features.push(`${index + 1}. **${attr}** - Carefully selected to enhance product performance`);
      });
    } else {
      features.push('1. **High Quality Construction** - Built to last with premium materials');
      features.push('2. **Reliable Performance** - Consistent results you can count on');
      features.push('3. **Easy Integration** - Seamlessly fits into your workflow');
    }
    
    features.push(`${features.length + 1}. **Quality Assurance** - Every ${productName} undergoes rigorous testing`);
    features.push(`${features.length + 1}. **Customer Support** - Backed by comprehensive warranty and support`);
    
    return features.join('\n');
  }

  /**
   * Generate benefits section
   * @private
   */
  private generateBenefits(productName: string, attributes: string[]): string {
    const benefits = [
      `The **${productName}** has been carefully designed to meet the highest standards of quality and performance.`,
      '',
      '**Benefits include:**',
      '- Superior quality and durability',
      '- Competitive pricing and value',
      '- Trusted by professionals worldwide',
      '- Comprehensive documentation and support'
    ];
    
    if (attributes.length > 3) {
      benefits.push('- Extensive feature set with multiple configuration options');
    }
    
    return benefits.join('\n');
  }

  /**
   * Generate applications section
   * @private
   */
  private generateApplications(attributes: string[]): string {
    const applications = [
      'This product is ideal for:',
      '- Commercial applications',
      '- Industrial settings',
      '- Professional use cases',
      '- Integration with existing systems',
      ''
    ];

    applications.push(
      attributes.length > 0
        ? `With its ${attributes.length} specialized attributes, this product adapts to various use cases and requirements.`
        : 'Versatile design allows for use across multiple industries and applications.'
    );

    return applications.join('\n');
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
        version: tool.version
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
