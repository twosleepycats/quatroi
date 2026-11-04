# Quất Roi

Đồ chơi desktop Windows nhỏ cho team: tay cầm roi bám con trỏ, click để quất và hiện tia lửa hoạt hình ở đầu roi kèm tiếng quất. Không chỉnh sửa nội dung cửa sổ bên dưới.

## Tải app

Tải **QuatRoi.exe** tại [GitHub Releases](https://github.com/twosleepycats/quatroi/releases/latest). Chạy trực tiếp, không cần tải source code hay cài đặt. File `SHA256SUMS.txt` đi kèm để kiểm tra tính toàn vẹn. Bản phát hành chưa ký số nên Windows có thể hiển thị cảnh báo nhà phát hành chưa xác định.

## Chạy

Mở `dist/QuatRoi.exe` trên Windows 10/11 có .NET Framework 4.8. App khởi động ẩn dưới khay hệ thống. Sprite được nhúng vào exe, không cần cài thêm thư viện. Không cần quyền admin.

App chỉ chạy một bản trong mỗi phiên Windows: mở exe thêm lần nữa sẽ thoát bản vừa mở, giữ nguyên app đang chạy. Dùng Ctrl+Shift+Home để hiện roi của bản đang chạy.

- Click trái trên bất kỳ màn hình nào: quất roi. Khi đang bật, app chặn click trái để tránh bấm nhầm ứng dụng phía dưới.
- `Ctrl + Shift + Home`: hiện roi; bấm lại khi đang hiện sẽ giữ nguyên.
- `Esc`: ẩn roi, dừng animation và âm thanh, trả chuột về bình thường. App vẫn chạy để bật lại bằng phím tắt.
- `F8`: phím bật/ẩn cũ vẫn được giữ.
- Chuột phải biểu tượng khay hệ thống: tạm dừng, tắt âm thanh hoặc thoát. Có thể cần mở mục biểu tượng ẩn trên taskbar.

Roi đi theo con trỏ trên tất cả màn hình, kể cả màn hình đặt bên trái hoặc phía trên màn hình chính. App dùng tọa độ pixel vật lý với nhận biết DPI từng màn hình; kích thước roi cố định 230 pixel. Hiệu ứng sát mép desktop có thể bị cắt để giữ tay cầm đúng vị trí con trỏ. Overlay dùng alpha từng pixel qua `UpdateLayeredWindow`, không dùng nền màu tím. Âm thanh WAV được nhúng vào exe và phát ở nhịp quất 280 ms. Nguồn CC0 và cách xử lý âm thanh nằm trong `assets/AUDIO-LICENSE.md`. Chưa đo mức dùng CPU/RAM thực tế.

## Build và kiểm tra

```powershell
powershell -ExecutionPolicy Bypass -File scripts/build.ps1
Start-Process .\dist\QuatRoi.exe -ArgumentList '--self-test' -Wait
Start-Process .\dist\QuatRoi.exe -ArgumentList '--smoke' -Wait
```

Build dùng C# compiler đi kèm .NET Framework trên Windows, không tải NuGet. Self-test kiểm tra atlas và xuất `art/preview.png`. Smoke test mở overlay khoảng 1,2 giây rồi đóng, có đọc trạng thái phím và cài mouse hook trong thời gian đó. Các phím tắt vẫn có thể được ứng dụng đang mở nhận cùng lúc. Để thoát hẳn, chọn **Thoat hoan toan** trong menu khay hệ thống.

Kiểm tra thủ công: mở app và xác nhận roi đang ẩn; Ctrl+Shift+Home để hiện; click ở giữa màn hình, bốn mép và click liên tiếp; Esc để ẩn và dùng chuột bình thường; Ctrl+Shift+Home để hiện lại. Bấm lặp tổ hợp khi đang hiện phải giữ nguyên trạng thái. Thử F8 và menu khay hệ thống, cuối cùng chọn thoát hoàn toàn. Kiểm tra trên cấu hình nhiều màn hình và DPI khác nhau trước khi chia sẻ rộng.

## Art

`assets/app-icon.png` là ảnh gốc icon cây roi. Build tự tạo `assets/app.ico` với các kích thước 16, 24, 32, 48, 64, 128 và 256 px, gắn vào file exe và biểu tượng khay hệ thống.

`assets/whip-atlas.png`: atlas PNG trong suốt 4 × 4 ô. Đọc trái sang phải, trên xuống dưới: frame 0–7 cây roi (70 ms/frame), frame 8–15 va chạm (35 ms/frame). Hiệu ứng bắt đầu ở 280 ms, kết thúc cùng cú quất ở 560 ms. Prompt và hướng dẫn art nằm trong `art/README.md`.

