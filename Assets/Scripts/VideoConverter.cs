using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using UnityEngine;
using System.Drawing;

public class VideoConverter:MonoBehaviour
{
    public async Task<MemoryStream> ConvertImagesToVideo(List<byte[]> images)
    {
        MemoryStream output = new MemoryStream();

        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.Out, 1,PipeTransmissionMode.Byte))
        {

        };

            ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Application.streamingAssetsPath, "ffmpeg.exe"), // FFmpeg�� ��θ� �˸°� �����ؾ� �մϴ�.
            Arguments = "-f image2pipe -i - -vcodec libx264 -pix_fmt yuv420p -movflags +faststart -y -",
            WindowStyle = ProcessWindowStyle.Normal,
            CreateNoWindow = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        //var processLog = new Progress<EventLogEntry>(entry =>
        //{
        //    logf
        //})

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();

            await Task.Run(() =>
            {
                foreach (byte[] image in images)
                {
                    process.StandardInput.BaseStream.Write(image, 0, image.Length);
                    process.StandardInput.BaseStream.Flush();
                }
                process.StandardInput.Close();
            });

            await process.StandardOutput.BaseStream.CopyToAsync(output);
            process.WaitForExit();
        }



        output.Position = 0;



        return output;
    }

    // �̹��� ��Ʈ���� �����ϴ� �޼���
    public MemoryStream SendImageStream(List<byte[]> imageStream)
    {
        var ffmpegPath = Path.Combine(Application.dataPath, "ffmpeg.exe"); // ffmpeg ���� ���� ���
        var ffmpegArgs = "-f image2pipe -i - -vcodec libx264 -pix_fmt yuv420p -movflags +faststart -y output.h264";
        // �̹����� �������� �Է�, �������κ��� �Է�������, �ڵ�����, ����������ȼ�, ����ȭ, ����� ���, �������ϸ�
        MemoryStream output = new MemoryStream();

        // ffmpeg ���μ��� ����
        var processInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = ffmpegArgs,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = false
        };

        using (var process = new Process())
        {
            process.StartInfo = processInfo;
            process.Start();

            // �̹��� ��Ʈ�� ����
            foreach (var imageData in imageStream)
            {
                process.StandardInput.BaseStream.Write(imageData, 0, imageData.Length);
                process.StandardInput.BaseStream.Flush();
            }

            // �̹��� ��Ʈ�� ����
            process.StandardInput.Close();

            process.StandardOutput.BaseStream.CopyTo(output);
            // ffmpeg �۾� �Ϸ� ���
            process.WaitForExit();
        }

        output.Position = 0;
        return output;
    }

    public void TempTest(List<byte[]> imageStream)
    {
        foreach (var imageData in imageStream)
        {
            Bitmap bitmap;
            using(var ms = new MemoryStream(imageData))
            {
                bitmap = new Bitmap(ms);
            }
        }
    }
}