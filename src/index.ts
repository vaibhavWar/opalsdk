/**
 * Optimizely Opal Custom Tool: Product Description Generator
 * 
 * This tool generates compelling product descriptions based on:
 * - Product Name
 * - Product Number (Part #)
 * - Product Attributes
 */

interface ProductDescriptionParams {
  productName: string;
  partNumber: string;
  attributes: string[];
}

interface ToolResponse {
  success: boolean;
  content?: string;
  error?: string;
  details?: string;
}

/**
 * Generates a product description using AI-friendly formatting
 */
async function generateProductDescription(params: ProductDescriptionParams): Promise<ToolResponse> {
  try {
    const { productName, partNumber, attributes } = params;

    // Validate required parameters
    if (!productName || !partNumber) {
      return {
        success: false,
        error: 'Missing required parameters',
        details: 'Both productName and partNumber are required'
      };
    }

    // Generate a comprehensive product description
    const description = createProductDescription(productName, partNumber, attributes);

    return {
      success: true,
      content: description
    };
  } catch (error) {
    return {
      success: false,
      error: 'Failed to generate product description',
      details: error instanceof Error ? error.message : 'Unknown error'
    };
  }
}

/**
 * Creates a formatted product description with markdown
 */
function createProductDescription(
  productName: string, 
  partNumber: string, 
  attributes: string[]
): string {
  // Build attribute list
  const attributeList = attributes && attributes.length > 0
    ? attributes.map(attr => `- ${attr}`).join('\n')
    : '- No additional attributes specified';

  // Generate description sections
  const sections = [
    '# Product Description',
    '',
    `## ${productName}`,
    '',
    `**Part Number:** \`${partNumber}\``,
    '',
    '---',
    '',
    '## Overview',
    '',
    generateOverview(productName, attributes),
    '',
    '## Key Features',
    '',
    generateKeyFeatures(productName, attributes),
    '',
    '## Technical Specifications',
    '',
    `**Product Name:** ${productName}  `,
    `**Part Number:** ${partNumber}`,
    '',
    '## Product Attributes',
    '',
    attributeList,
    '',
    '---',
    '',
    '## Why Choose This Product?',
    '',
    generateWhyChoose(productName, attributes),
    '',
    '## Product Applications',
    '',
    generateApplications(attributes),
    '',
    '---',
    '',
    `*Generated product description for ${productName} (Part #: ${partNumber})*`
  ];

  return sections.join('\n');
}

/**
 * Generates an overview section based on product information
 */
function generateOverview(productName: string, attributes: string[]): string {
  const baseOverview = `The **${productName}** is a premium product designed to deliver exceptional performance and reliability. `;
  
  let overview = baseOverview;
  
  if (attributes && attributes.length > 0) {
    overview += `This product features ${attributes.length} key attribute${attributes.length > 1 ? 's' : ''} that make it stand out in its category. `;
  }
  
  overview += `Engineered with precision and built to last, the ${productName} represents the perfect balance of quality, functionality, and value.`;
  
  return overview;
}

/**
 * Generates key features based on product attributes
 */
function generateKeyFeatures(productName: string, attributes: string[]): string {
  const features: string[] = [];
  
  // Add attribute-based features
  if (attributes && attributes.length > 0) {
    attributes.forEach((attr, index) => {
      features.push(`${index + 1}. **${attr}** - Carefully selected to enhance product performance`);
    });
  } else {
    // Default features when no attributes provided
    features.push('1. **High Quality Construction** - Built to last with premium materials');
    features.push('2. **Reliable Performance** - Consistent results you can count on');
    features.push('3. **Easy Integration** - Seamlessly fits into your workflow');
  }
  
  // Add standard features
  features.push(`${features.length + 1}. **Quality Assurance** - Every ${productName} undergoes rigorous testing`);
  features.push(`${features.length + 1}. **Customer Support** - Backed by comprehensive warranty and support`);
  
  return features.join('\n');
}

/**
 * Generates "Why Choose" section
 */
function generateWhyChoose(productName: string, attributes: string[]): string {
  const reasons = [
    `The **${productName}** has been carefully designed to meet the highest standards of quality and performance.`,
    '',
    '**Benefits include:**',
    '- Superior quality and durability',
    '- Competitive pricing and value',
    '- Trusted by professionals worldwide',
    '- Comprehensive documentation and support'
  ];
  
  if (attributes && attributes.length > 3) {
    reasons.push('- Extensive feature set with multiple configuration options');
  }
  
  return reasons.join('\n');
}

/**
 * Generates applications section based on attributes
 */
function generateApplications(attributes: string[]): string {
  return [
    'This product is ideal for:',
    '- Commercial applications',
    '- Industrial settings',
    '- Professional use cases',
    '- Integration with existing systems',
    '',
    attributes && attributes.length > 0
      ? `With its ${attributes.length} specialized attributes, this product adapts to various use cases and requirements.`
      : 'Versatile design allows for use across multiple industries and applications.'
  ].join('\n');
}

/**
 * Main request handler for Cloudflare Workers
 */
export default {
  async fetch(request: Request): Promise<Response> {
    const url = new URL(request.url);

    // CORS headers for all responses
    const corsHeaders = {
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
      'Access-Control-Allow-Headers': 'Content-Type, Authorization',
    };

    // Handle CORS preflight requests
    if (request.method === 'OPTIONS') {
      return new Response(null, {
        headers: corsHeaders,
      });
    }

    // Discovery endpoint - describes the tool to Optimizely Opal
    if (request.method === 'GET' && url.pathname === '/discovery') {
      const discoveryResponse = {
        functions: [
          {
            name: 'product-description-generator',
            description: 'Generates comprehensive product descriptions based on product name, part number, and attributes. Creates marketing-ready content with structured sections including overview, features, specifications, and applications.',
            parameters: [
              {
                name: 'productName',
                type: 'string',
                description: 'The name of the product',
                required: true
              },
              {
                name: 'partNumber',
                type: 'string',
                description: 'The product part number or SKU',
                required: true
              },
              {
                name: 'attributes',
                type: 'array',
                description: 'List of product attributes, features, or specifications (e.g., ["Color: Blue", "Size: Large", "Material: Steel"])',
                required: false
              }
            ],
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

    // Health check endpoint
    if (request.method === 'GET' && url.pathname === '/') {
      return new Response(JSON.stringify({
        status: 'healthy',
        tool: 'product-description-generator',
        version: '1.0.0',
        description: 'Optimizely Opal tool for generating product descriptions'
      }, null, 2), {
        headers: {
          'Content-Type': 'application/json',
          ...corsHeaders,
        },
      });
    }

    // Tool execution endpoint
    if (request.method === 'POST' && url.pathname === '/') {
      try {
        const body = await request.json();
        
        // Handle different possible parameter formats from Optimizely Opal
        let params: ProductDescriptionParams;
        const bodyObj = body as any;

        // Check if parameters are nested under 'parameters' key
        if (bodyObj.parameters) {
          params = {
            productName: bodyObj.parameters.productName,
            partNumber: bodyObj.parameters.partNumber,
            attributes: bodyObj.parameters.attributes || []
          };
        } 
        // Check if parameters are in 'arguments' key
        else if (bodyObj.arguments) {
          params = {
            productName: bodyObj.arguments.productName,
            partNumber: bodyObj.arguments.partNumber,
            attributes: bodyObj.arguments.attributes || []
          };
        }
        // Check if parameters are directly in the body
        else if (bodyObj.productName) {
          params = {
            productName: bodyObj.productName,
            partNumber: bodyObj.partNumber,
            attributes: bodyObj.attributes || []
          };
        } else {
          return new Response(JSON.stringify({
            success: false,
            error: 'Invalid request format',
            details: 'Expected productName and partNumber in request body'
          }), {
            status: 400,
            headers: {
              'Content-Type': 'application/json',
              ...corsHeaders,
            },
          });
        }

        // Generate the product description
        const result = await generateProductDescription(params);

        return new Response(JSON.stringify(result, null, 2), {
          status: result.success ? 200 : 400,
          headers: {
            'Content-Type': 'application/json',
            ...corsHeaders,
          },
        });
      } catch (error) {
        return new Response(JSON.stringify({
          success: false,
          error: 'Invalid request format',
          details: error instanceof Error ? error.message : 'Unknown error'
        }), {
          status: 400,
          headers: {
            'Content-Type': 'application/json',
            ...corsHeaders,
          },
        });
      }
    }

    // Route not found
    return new Response(JSON.stringify({
      error: 'Not Found',
      message: 'Available endpoints: GET /discovery, GET / (health), POST / (generate)'
    }), {
      status: 404,
      headers: {
        'Content-Type': 'application/json',
        ...corsHeaders,
      },
    });
  },
};

