# Additional weapons

Generated with the built-in `image_gen` tool. Final project assets: `assets/sword-atlas.png` and `assets/hammer-atlas.png`. Each uses a 4 × 4 atlas: first eight cells are the swing, last eight cells are impact VFX. Runtime mirrors weapon frames to match the original left-facing whip; VFX stays unmirrored.

## Sword prompt

Use case: stylized-concept. Production game sprite atlas PNG, square image with genuine transparent background everywhere. Exactly 4 equal columns x 4 equal rows, no gaps, no grid lines or text. 16 cells. First 8 cells (top 2 rows): same friendly cartoon short silver sword with chunky blue handle and gold guard, eight consecutive swing animation frames: idle, windup, raised, begin swing, fast swing, full slash, recover, idle. IMPORTANT identical grip pivot at (20% from left, 80% from top) inside EVERY cell, with entire blade safely inside cell; blade swings mostly to right. Same scale, crisp bold dark outlines. Bottom 8 cells: eight consecutive blue-cyan comic slash impact effects centered in each cell, from tiny glint to crescent swoosh to bright peak to fading sparks. All sprites fully within cells with padding. Cute desktop toy, no people no injury no blood. Transparent alpha outside sprites including bottom rows, absolutely no white or colored backdrop or checkerboard.

Cleanup edit: remove diffuse gray-blue fog, haze, glow, shadows and atmospheric background; retain crisp swords, cyan crescents and star sparks, with transparent alpha around them. Preserve layout and sprite positions.

## Hammer prompt

Use case: stylized-concept. Production sprite atlas for a cute Windows desktop hammer toy. One square PNG with genuine transparent background. Exactly 4 equal columns and 4 equal rows, NO borders, NO labels. Every sprite stays in its own cell with 10% padding. First 8 cells, reading left to right: same chunky wooden toy mallet with orange handle and oversized rounded wooden head, 8 successive swing poses idle, backswing, raised, begin downswing, fast downswing, impact horizontal toward right, recovery, idle. Same scale and grip pivot at 20% left /80% top in each cell. Thick dark outlines, warm brown amber palette, simple cute cartoon game art. Last 8 cells: eight successive comic impact VFX centered in cells: small gold glint, compact impact, yellow starburst, large orange starburst with small dust puff, expanding dust ring, scattered dust and gold sparks, fading dust, final few sparks. Actual transparent alpha everywhere outside sprites and inside gaps, no gray/white backdrop, no people, no blood, no text, no checkerboard.

## Sound

Sword swish and hammer thump are original deterministic synthesized PCM effects generated in `src/Weapon.cs`. The whip retains the credited CC0 recording.
