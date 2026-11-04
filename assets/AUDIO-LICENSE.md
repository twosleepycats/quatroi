# Whip crack audio

Sword swish and hammer thump in v1.1.0 are original synthesized effects generated in `src/Weapon.cs`; no additional third-party audio is used for those weapons.

- Title: Whip crack #1 (sound 2949)
- Author: Joseph SARDIN
- Source and license statement: https://bigsoundbank.com/whip-crack-1-s2949.html
- Original download: https://bigsoundbank.com/UPLOAD/mp3/2949.mp3
- License: CC0 (public domain), as stated on the source page. Editing and redistribution in apps are permitted.
- Retrieved: 2026-11-04
- Original: `whip-source.mp3`
- Runtime derivative: `whip-crack.wav`, mono 44.1 kHz, 16-bit PCM, 0.45 seconds. Trimmed at 0.245 seconds, volume scaled to 65%, final 0.13 seconds faded out. Embedded into the executable.

Recreate the derivative with FFmpeg:

```powershell
ffmpeg -y -ss 0.245 -i assets/whip-source.mp3 -t 0.45 -af "volume=0.65,afade=t=out:st=0.32:d=0.13" -ar 44100 -ac 1 -c:a pcm_s16le assets/whip-crack.wav
```
