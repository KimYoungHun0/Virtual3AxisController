using System;
using System.Drawing;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Virtual3AxisController
{
    public class VisionManager
    {
        private VideoCapture _capture;
        private Mat _frame;
        private Thread _cameraThread;
        private bool _isCameraRunning = false;

        // UI 이벤트
        public event Action<Bitmap>? OnFrameReady;
        public event Action<string>? OnCameraError;
        // ★ 비전 알고리즘에서 찾은 타깃의 중심 좌표(X, Y)를 전달하는 이벤트
        public event Action<int, int>? OnTargetDetected;

        public void StartCamera(int cameraIndex = 0)
        {
            if (_isCameraRunning) return;

            _capture = new VideoCapture(cameraIndex);
            _capture.Set(VideoCaptureProperties.FrameWidth, 320);
            _capture.Set(VideoCaptureProperties.FrameHeight, 240);

            if (!_capture.IsOpened())
            {
                OnCameraError?.Invoke("웹캠을 찾을 수 없거나 이미 사용 중입니다.");
                return;
            }

            _frame = new Mat();
            _isCameraRunning = true;

            _cameraThread = new Thread(CameraLoop);
            _cameraThread.IsBackground = true;
            _cameraThread.Start();
        }

        public void StopCamera()
        {
            _isCameraRunning = false;
            if (_cameraThread != null && _cameraThread.IsAlive)
            {
                _cameraThread.Join(500);
            }
            _capture?.Dispose();
            _frame?.Dispose();
        }

        private void CameraLoop()
        {
            while (_isCameraRunning)
            {
                try
                {
                    if (_capture.Read(_frame) && !_frame.Empty())
                    {
                        // ====================================================
                        // ★ OpenCV 비전 알고리즘: 파란색 물체 추적 (Blue Color Tracking)
                        // ====================================================
                        // Mat 메모리 누수 방지를 위해 using 블록 사용 (매우 중요)
                        using (Mat hsv = new Mat())
                        using (Mat mask = new Mat())
                        {
                            // 1. 색상 추적을 위해 BGR 포맷을 HSV 포맷으로 변환
                            Cv2.CvtColor(_frame, hsv, ColorConversionCodes.BGR2HSV);

                            // 2. 파란색 영역 지정 (주변 빛에 따라 값 조정 필요)
                            Scalar lowerBlue = new Scalar(100, 100, 100);
                            Scalar upperBlue = new Scalar(130, 255, 255);
                            Cv2.InRange(hsv, lowerBlue, upperBlue, mask);

                            // 3. 자잘한 노이즈 제거 (모폴로지 연산)
                            using (Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(5, 5)))
                            {
                                Cv2.MorphologyEx(mask, mask, MorphTypes.Open, kernel);
                            }

                            // 4. 물체의 윤곽선(Contour) 검출
                            Cv2.FindContours(mask, out OpenCvSharp.Point[][] contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                            if (contours.Length > 0)
                            {
                                // 가장 큰 영역(노이즈 제외) 찾기
                                double maxArea = 0;
                                int maxIndex = -1;
                                for (int i = 0; i < contours.Length; i++)
                                {
                                    double area = Cv2.ContourArea(contours[i]);
                                    if (area > maxArea && area > 500) // 500픽셀 이상의 크기만 인정
                                    {
                                        maxArea = area;
                                        maxIndex = i;
                                    }
                                }

                                // 5. 대상 물체를 찾았다면 중심 좌표 계산
                                if (maxIndex != -1)
                                {
                                    Moments moments = Cv2.Moments(contours[maxIndex]);
                                    if (moments.M00 > 0)
                                    {
                                        int cx = (int)(moments.M10 / moments.M00);
                                        int cy = (int)(moments.M01 / moments.M00);

                                        // 영상 위에 십자선과 좌표 텍스트 그리기
                                        Cv2.DrawContours(_frame, contours, maxIndex, Scalar.LimeGreen, 2);
                                        Cv2.Circle(_frame, cx, cy, 5, Scalar.Red, -1);
                                        Cv2.PutText(_frame, $"Target: {cx}, {cy}", new OpenCvSharp.Point(cx + 10, cy - 10), HersheyFonts.HersheySimplex, 0.5, Scalar.Red, 1);

                                        // UI로 좌표 데이터 전송
                                        OnTargetDetected?.Invoke(cx, cy);
                                    }
                                }
                            }
                        }

                        // 결과 이미지를 UI로 전송
                        var bitmap = BitmapConverter.ToBitmap(_frame);
                        OnFrameReady?.Invoke(bitmap);
                    }
                    Thread.Sleep(30);
                }
                catch (Exception ex)
                {
                    OnCameraError?.Invoke($"비전 루프 에러: {ex.Message}");
                    break;
                }
            }
        }
    }
}