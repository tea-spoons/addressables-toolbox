namespace TeaSpoons.AddressablesToolbox
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UI;

    public static class ImageExtensions
    {
        /// <summary>
        /// Loads a <see cref="Sprite"/> from the supplied <see cref="SmartAssetReference{T}"/>,
        /// in the background if there is a valid reference, and assigns it to the target <see cref="Image"/>.
        /// </summary>
        public static void LoadImage(this Image image, SmartAssetReference<Sprite> assetReference)
        {
            if (assetReference is null || !assetReference.HasValidReference)
            {
                return;
            }

            image.LoadImageAsync(assetReference).Forget();
        }

        private static async UniTask LoadImageAsync(this Image image, SmartAssetReference<Sprite> assetReference)
        {
            var sprite = await assetReference.GetAssetAsync();

            if (image)
            {
                image.sprite = sprite;
            }
        }
    }
}