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
            FileName = Path.Combine(Application.streamingAssetsPath, "ffmpeg.exe"), // FFmpeg의 경로를 알맞게 설정해야 합니다.
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

    // 이미지 스트림을 전송하는 메서드
    public MemoryStream SendImageStream(List<byte[]> imageStream)
    {
        var ffmpegPath = Path.Combine(Application.dataPath, "ffmpeg.exe"); // ffmpeg 실행 파일 경로
        var ffmpegArgs = "-f image2pipe -i - -vcodec libx264 -pix_fmt yuv420p -movflags +faststart -y output.h264";
        // 이미지를 파이프로 입력, 파이프로부터 입력을받음, 코덱셜정, 출력파일의픽셀, 최적화, 덮어쓰기 허용, 최종파일명
        MemoryStream output = new MemoryStream();

        // ffmpeg 프로세스 생성
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

            // 이미지 스트림 전송
            foreach (var imageData in imageStream)
            {
                process.StandardInput.BaseStream.Write(imageData, 0, imageData.Length);
                process.StandardInput.BaseStream.Flush();
            }

            // 이미지 스트림 종료
            process.StandardInput.Close();

            process.StandardOutput.BaseStream.CopyTo(output);
            // ffmpeg 작업 완료 대기
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