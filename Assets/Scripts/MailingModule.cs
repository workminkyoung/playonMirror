using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using UnityEngine;
using System.IO;

public class MailingModule : SingletonBehaviour<MailingModule>
{
    public void SendMail(MAIL_TYPE type)
    {
        foreach (var elem in AdminManager.Instance.BasicSetting.Device[LogDataManager.Instance.GetGuid].mailList_data)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient("mail.vivestudios.com");
            mail.From = new MailAddress("playon@vivestudios.com");
            //foreach (var elem in ConfigData.config.mailConfig.mailingList)
            mail.To.Add(elem);
            switch (type)
            {
                case MAIL_TYPE.REMAIN_PAPER:
                    mail.Subject = "[플레이온] 인화지 잔여 수량 알림";
                    mail.Body = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Mailing", "MailFormat_RemainPaper.html")).Replace("{0}", AdminManager.Instance.BasicSetting.Device[LogDataManager.Instance.GetGuid].StoreName).Replace("{1}", PhotoPaperCheckModule.GetRemainPhotoPaper().ToString());
                    break;
                case MAIL_TYPE.RESET_PAPER:
                    mail.Subject = "[플레이온] 인화지 잔여 수량 초기화 알림";
                    mail.Body = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Mailing", "MailFormat_ResetPaper.html")).Replace("{0}", AdminManager.Instance.BasicSetting.Device[LogDataManager.Instance.GetGuid].StoreName).Replace("{1}", PhotoPaperCheckModule.GetRemainPhotoPaper().ToString());
                    break;
                case MAIL_TYPE.ERROR:
                    mail.Subject = "[플레이온] 장비 진단 필요";
                    mail.Body = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Mailing", "MailFormat_Error.html")).Replace("{0}", AdminManager.Instance.BasicSetting.Device[LogDataManager.Instance.GetGuid].StoreName).Replace("{1}", !GameManager.inst.isCameraConnected ? "카메라 연결 상태 점검" : !GameManager.inst.isPaymentReaderConnected ? "결제 리더기 연결 상태 점검" : "알 수 없는 오류, 확인 바랍니다.");
                    break;
                case MAIL_TYPE.PAYMENT_ERROR:
                    mail.Subject = "[플레이온] PG 통신 진단";
                    mail.Body = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Mailing", "MailFormat_PaymentError.html")).Replace("{0}", AdminManager.Instance.BasicSetting.Device[LogDataManager.Instance.GetGuid].StoreName).Replace("{1}", PaymentModule.inst.ErrorContent);
                    break;
                case MAIL_TYPE.DIFFUSION_ERROR:
                    mail.Subject = "[플레이온] AI 이미지 생성 실패";
                    mail.Body = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Mailing", "MailFormat_DiffusionError.html")).Replace("{0}", AdminManager.Instance.BasicSetting.Device[LogDataManager.Instance.GetGuid].StoreName).Replace("{1}", PaymentModule.inst.ErrorContent);
                    break;
                case MAIL_TYPE.QR_ERROR:
                    mail.Subject = "[플레이온] QR 생성용 서버 업로드 실패";
                    mail.Body = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Mailing", "MailFormat_QRError.html")).Replace("{0}", AdminManager.Instance.BasicSetting.Device[LogDataManager.Instance.GetGuid].StoreName).Replace("{1}", PaymentModule.inst.ErrorContent);
                    break;
            }

            smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpServer.Port = 25;
            smtpServer.Credentials = (ICredentialsByHost)new NetworkCredential("playon@vivestudios.com", "vive1234");
            smtpServer.Timeout = 20000;
            try
            {
                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError(ex.Message);
            }
        }
    }

    protected override void Init()
    {
    }
}
