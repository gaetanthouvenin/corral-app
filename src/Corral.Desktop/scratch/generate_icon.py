from PIL import Image, ImageDraw, ImageFont
import os

size = 256
img = Image.new('RGBA', (size, size), (0, 0, 0, 0))
d = ImageDraw.Draw(img)

# Background squircle
d.rounded_rectangle([16, 16, 240, 240], radius=48, fill=(28, 28, 30, 255))

# Draw 2x2 grid of zones (fences)
# Top-left box: Blue accent
d.rounded_rectangle([48, 48, 116, 116], radius=20, fill=(26, 128, 229, 255))
# Top-right box
d.rounded_rectangle([140, 48, 208, 116], radius=20, fill=(60, 60, 64, 255))
# Bottom-left box
d.rounded_rectangle([48, 140, 116, 208], radius=20, fill=(60, 60, 64, 255))
# Bottom-right box
d.rounded_rectangle([140, 140, 208, 208], radius=20, fill=(60, 60, 64, 255))

assets_dir = r'c:\Users\gatan\Sources\GitHub\corral-app\src\Corral.Desktop\Assets'
os.makedirs(assets_dir, exist_ok=True)
img.save(os.path.join(assets_dir, 'app_icon.ico'), format='ICO', sizes=[(256, 256), (128, 128), (64, 64), (32, 32), (16, 16)])
print('Saved app_icon.ico')
