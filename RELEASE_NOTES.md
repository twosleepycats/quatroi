# Quất Roi v1.0.0

Đồ chơi desktop Windows: roi hoạt hình bám con trỏ, hiệu ứng tiếp xúc và tiếng quất vui vẻ cho team.

## Tải và sử dụng

1. Tải `QuatRoi.exe` trong mục Assets bên dưới.
2. Chạy trực tiếp trên Windows 10/11 có .NET Framework 4.8. Không cần cài thêm thư viện hay quyền admin.
3. Nhấn **Ctrl + Shift + Home** để hiện roi, click trái để quất.
4. Nhấn **Esc** để ẩn roi và dùng chuột bình thường.
5. Chuột phải biểu tượng khay hệ thống để tắt âm thanh hoặc **Thoat hoan toan**.

App khởi động ẩn. Hoạt động trên nhiều màn hình; mở file nhiều lần chỉ giữ một app trong phiên Windows. F8 cũng có thể bật/ẩn roi.

Khi roi đang bật, click trái được app giữ lại để tránh bấm nhầm ứng dụng phía dưới. Bản exe chưa ký số; Windows có thể cảnh báo nhà phát hành chưa xác định. Hiệu ứng có thể bị cắt ở mép desktop; chưa kiểm tra thủ công trên mọi cấu hình DPI.

## Kiểm tra file tải về

So sánh kết quả với `SHA256SUMS.txt`:

```powershell
Get-FileHash .\QuatRoi.exe -Algorithm SHA256
```

Âm thanh: Whip crack #1 của Joseph SARDIN / BigSoundBank, CC0. Chi tiết trong `assets/AUDIO-LICENSE.md` của repository.
