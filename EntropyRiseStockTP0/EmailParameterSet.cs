using System;
using System.Net;
using System.Net.Mail;
using System.Text;
public class EmailParameterSet
{
    /// <summary>
    /// 收件人的邮件地址 
    /// </summary>
    public string ConsigneeAddress { get; set; }

    /// <summary>
    /// 收件人的名称
    /// </summary>
    public string ConsigneeName { get; set; }

    /// <summary>
    /// 收件人标题
    /// </summary>
    public string ConsigneeHand { get; set; }

    /// <summary>
    /// 收件人的主题
    /// </summary>
    public string ConsigneeTheme { get; set; }

    /// <summary>
    /// 发件邮件服务器的Smtp设置
    /// </summary>
    public string SendSetSmtp { get; set; }

    /// <summary>
    /// 发件人的邮件
    /// </summary>
    public string SendEmail { get; set; }

    /// <summary>
    /// 发件人的邮件密码
    /// </summary>
    public string SendPwd { get; set; }
    /// <summary>
    /// 发件内容
    /// </summary>
    public string SendContent { get; set; }

    public bool MailSend(EmailParameterSet EPSModel,out string errinfo)
    {
        errinfo = "";
        try
        {
            //确定smtp服务器端的地址，实列化一个客户端smtp 
            System.Net.Mail.SmtpClient sendSmtpClient = new System.Net.Mail.SmtpClient(EPSModel.SendSetSmtp);//发件人的邮件服务器地址
            //构造一个发件的人的地址
            System.Net.Mail.MailAddress sendMailAddress = new MailAddress(EPSModel.SendEmail, EPSModel.ConsigneeHand, Encoding.UTF8);//发件人的邮件地址和收件人的标题、编码

            //构造一个收件的人的地址
            System.Net.Mail.MailAddress consigneeMailAddress = new MailAddress(EPSModel.ConsigneeAddress, EPSModel.ConsigneeAddress, Encoding.UTF8);//收件人的邮件地址和收件人的名称 和编码

            //构造一个Email对象
            System.Net.Mail.MailMessage mailMessage = new MailMessage(sendMailAddress, consigneeMailAddress);//发件地址和收件地址
            mailMessage.Subject = EPSModel.ConsigneeTheme;//邮件的主题
            mailMessage.BodyEncoding = Encoding.UTF8;//编码
            mailMessage.SubjectEncoding = Encoding.UTF8;//编码
            mailMessage.Body = EPSModel.SendContent;//发件内容
            mailMessage.IsBodyHtml = false;//获取或者设置指定邮件正文是否为html

            //设置邮件信息 (指定如何处理待发的电子邮件)
            sendSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定如何发邮件 是以网络来发
            sendSmtpClient.EnableSsl = false;//服务器支持安全接连，安全则为true

            sendSmtpClient.UseDefaultCredentials = false;//是否随着请求一起发

            //用户登录信息
            NetworkCredential myCredential = new NetworkCredential(EPSModel.SendEmail, EPSModel.SendPwd);
            sendSmtpClient.Credentials = myCredential;//登录
            sendSmtpClient.Send(mailMessage);//发邮件
            return true;//发送成功
        }
        catch (Exception e)
        {
            errinfo = e.Message;
            return false;//发送失败
        }
    }
}