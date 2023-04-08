using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace RssFeeder.Server.Infrastructure.Services.Implementations
{
    public class ExtractImage : IExtractImage
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const int MAX_ALLOWED_WIDTH = 120;

        public ExtractImage(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetImageBase64ByHref(string href)
        {
            var httpClient = _httpClientFactory.CreateClient();

            HttpResponseMessage response = await httpClient.GetAsync(href);
            string html = await response.Content.ReadAsStringAsync();

            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);

            if (document is null || document.Head is null) return string.Empty;

            var previewImageUrl = GetPreviewImageUrl(document);

            if (!string.IsNullOrEmpty(previewImageUrl))
            {
                // Download the preview image and resize it
                using (var imageStream = await httpClient.GetStreamAsync(previewImageUrl))
                {
                    return await ResizeImage(imageStream);
                }
            }

            return string.Empty;
        }

        private string GetPreviewImageUrl(IHtmlDocument document)
        {
            // Try to extract the preview image URL using the 'og:image' meta tag
            var ogImageNode = document!.Head!.QuerySelector("meta[property='og:image']");
            string previewImageUrl = ogImageNode?.GetAttribute("content");

            // If the 'og:image' meta tag is not present, try the 'twitter:image' meta tag
            if (previewImageUrl == null)
            {
                var twitterImageNode = document.Head.QuerySelector("meta[property='twitter:image']");
                previewImageUrl = twitterImageNode?.GetAttribute("content");
            }

            // If the 'twitter:image' meta tag is not present, try the 'link[rel='image_src']' tag
            if (previewImageUrl == null)
            {
                var imageSrcNode = document.Head.QuerySelector("link[rel='image_src']");
                previewImageUrl = imageSrcNode?.GetAttribute("href");
            }

            // If none of the above methods work, try selecting the first 'img' tag with a 'src' attribute
            if (previewImageUrl == null)
            {
                var firstImageNode = document.QuerySelector("img[src]");
                previewImageUrl = firstImageNode?.GetAttribute("src");
            }

            return previewImageUrl;
        }

        private async Task<string> ResizeImage(Stream imageStream)
        {
            using (var image = await Image.LoadAsync(imageStream))
            {
                int maxWidth = MAX_ALLOWED_WIDTH;

                if (image.Width > maxWidth)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(maxWidth, 0),
                        Mode = ResizeMode.Max
                    }));
                }

                // Save the resized image to a file or stream
                // For example, you can save it to a file like this:
                // image.Save("preview.jpg", new JpegEncoder());

                // Convert the resized image to a Base64 string
                var memoryStream = new MemoryStream();
                image.Save(memoryStream, new JpegEncoder());
                var base64String = Convert.ToBase64String(memoryStream.ToArray());

                return base64String;
            }
        }
    }
}
