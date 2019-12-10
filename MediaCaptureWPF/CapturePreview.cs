﻿using MediaCaptureWPF.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;

namespace MediaCaptureWPF
{
    public class CapturePreview : D3DImage
    {
        CapturePreviewNative m_preview;
        MediaCapture m_capture;
        uint m_width;
        uint m_height;

        public CapturePreview(MediaCapture capture, VideoRotation rotation = VideoRotation.None)
        {
            capture.SetPreviewRotation(rotation);
            var props = (VideoEncodingProperties)capture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            if (rotation == VideoRotation.None || rotation == VideoRotation.Clockwise180Degrees)
            {
                m_width = props.Width;
                m_height = props.Height;
            }
            else
            {
                m_width = props.Height;
                m_height = props.Width;
            }

            m_preview = new CapturePreviewNative(this, m_width, m_height);
            m_capture = capture;
        }

        public async Task StartAsync()
        {
            var profile = new MediaEncodingProfile
            {
                Audio = null,
                Video = VideoEncodingProperties.CreateUncompressed(MediaEncodingSubtypes.Rgb32, m_width, m_height),
                Container = null
            };

            await m_capture.StartPreviewToCustomSinkAsync(profile, (IMediaExtension)m_preview.MediaSink);
        }

        public async Task StopAsync()
        {
            await m_capture.StopPreviewAsync();
        }
    }
}
