using FaceRecognitionDotNet;
using System.Drawing;

namespace Wasalnyy.BLL.Service.Implementation
{
	public class FaceService : IFaceService
	{
		private readonly FaceRecognition _faceRecognition;
		public FaceService(FaceRecognition faceRecognition) => _faceRecognition = faceRecognition;

		public double[] ExtractEmbedding(byte[] imageBytes)
		{
			using var ms = new MemoryStream(imageBytes);
			using var bmp = new Bitmap(ms);
			using var img = FaceRecognition.LoadImage(bmp);

			var encodings = _faceRecognition.FaceEncodings(img).ToList();
			if (!encodings.Any())
				throw new Exception("No face detected");

			return encodings[0].GetRawEncoding();
		}

		public double CompareEmbeddings(double[] emb1, double[] emb2)
		{
			double sum = 0;
			for (int i = 0; i < emb1.Length; i++)
				sum += Math.Pow(emb1[i] - emb2[i], 2);
			return Math.Sqrt(sum);
		}
	}
}
