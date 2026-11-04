using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Reflection;

namespace QuatRoi
{
    internal sealed class Weapon : IDisposable
    {
        internal readonly string Name;
        internal readonly Bitmap Atlas;
        internal readonly SoundPlayer Sound;
        internal readonly Point ImpactOffset;
        internal readonly double GripX;
        internal readonly double GripY;
        private readonly Stream audio;

        private Weapon(string name, string resource, double gripX, double gripY, Point impact, int tone)
        {
            Name = name;
            GripX = gripX;
            GripY = gripY;
            ImpactOffset = impact;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            using (var image = new Bitmap(stream)) Atlas = new Bitmap(image);
            audio = tone == 0 ? Assembly.GetExecutingAssembly().GetManifestResourceStream("whip-crack.wav") : CreateSound(tone);
            Sound = new SoundPlayer(audio);
            Sound.Load();
        }

        internal static Weapon[] LoadAll()
        {
            return new[] {
                new Weapon("Roi", "whip-atlas.png", 0.17, 0.85, new Point(-168, -12), 0),
                new Weapon("Kiếm", "sword-atlas.png", 0.20, 0.78, new Point(-156, 0), 1),
                new Weapon("Búa", "hammer-atlas.png", 0.26, 0.83, new Point(-115, -20), 2)
            };
        }

        internal Rectangle Bounds(Point cursor, int size)
        {
            return new Rectangle(cursor.X - (int)(size * (1 - GripX)), cursor.Y - (int)(size * GripY), size, size);
        }

        // Short original synthesized effects: airy sword swish and low wooden thump.
        private static Stream CreateSound(int kind)
        {
            const int rate = 22050;
            int count = (int)(rate * 0.28);
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + count * 2);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
            writer.Write(16); writer.Write((short)1); writer.Write((short)1);
            writer.Write(rate); writer.Write(rate * 2); writer.Write((short)2); writer.Write((short)16);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data")); writer.Write(count * 2);
            var random = new Random(245 + kind);
            double filtered = 0;
            for (int i = 0; i < count; i++)
            {
                double time = (double)i / rate;
                double noise = random.NextDouble() * 2 - 1;
                filtered = filtered * 0.85 + noise * 0.15;
                double value = kind == 1
                    ? (noise - filtered) * Math.Sin(Math.PI * i / count) * Math.Exp(-time * 13) * 0.3
                    : (Math.Sin(2 * Math.PI * (125 * time - 110 * time * time)) * Math.Exp(-time * 25) * 0.5 + filtered * Math.Exp(-time * 35) * 0.4);
                double fadeIn = Math.Min(1, i / 70.0);
                writer.Write((short)(Math.Max(-1, Math.Min(1, value * fadeIn)) * short.MaxValue));
            }
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public void Dispose()
        {
            Sound.Stop(); Sound.Dispose(); audio.Dispose(); Atlas.Dispose();
        }
    }
}
