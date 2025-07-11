# Azure Static Website

This is a simple static website designed to be hosted on Azure Static Web Apps.

## Website Structure

- `index.html` - Home page
- `services.html` - Services page
- `about.html` - About Us page
- `contact.html` - Contact page

## How to Deploy to Azure Static Web Apps

### Prerequisites

- An Azure account
- Visual Studio Code with the Azure Static Web Apps extension (optional but recommended)
- Git installed on your machine

### Deployment Steps

#### Option 1: Using Visual Studio Code

1. Install the Azure Static Web Apps extension for VS Code
2. Sign in to your Azure account within VS Code
3. Open the Azure extension and click on the "+" to create a new Static Web App
4. Follow the prompts to:
   - Select your Azure subscription
   - Enter a name for your static web app
   - Choose a region
   - Select a GitHub repository (if your code is hosted on GitHub)
   - Set the build details (for a simple static site, you can leave the defaults)
5. Wait for the deployment to complete

#### Option 2: Using Azure Portal

1. Log in to the [Azure Portal](https://portal.azure.com)
2. Search for "Static Web Apps" and select it
3. Click "Create"
4. Fill in the required details:
   - Subscription
   - Resource Group
   - Name
   - Hosting plan (Free or Standard)
   - Region
   - Source (GitHub, Azure DevOps, or Other)
5. Configure your deployment source:
   - For GitHub, authorize Azure and select your repository, branch
   - For manual deployment, complete the creation then use the deployment tools to upload your files
6. Review and create
7. Wait for the deployment to complete

#### Option 3: Manual Upload to Azure Storage

If you're using Azure Storage for static website hosting:

1. Log in to the [Azure Portal](https://portal.azure.com)
2. Navigate to your Storage Account
3. Click on "Static website" in the left menu
4. Enable static website hosting
5. Set "index.html" as the index document name
6. Set "error.html" as the error document path
7. Save the changes
8. Upload your website files to the "$web" container using:
   - Azure Portal storage browser
   - Azure Storage Explorer
   - AzCopy command-line tool
   - Azure CLI

## Customizing the Website

This website is built with pure HTML, CSS, and minimal JavaScript. To customize:

1. Edit the HTML files to update content
2. Modify the CSS within the `<style>` tags to change the appearance
3. Add your own images to replace the icon placeholders
4. Update links and contact information to match your business details

## Additional Resources

- [Azure Static Web Apps documentation](https://docs.microsoft.com/en-us/azure/static-web-apps/)
- [Azure Storage static website hosting](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-static-website)
