using CShellNet;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace OKX_API;

public class Notification
{
    private string title = "";
    private string msg = "";
    private CShell shell = new CShell();
    public Notification(string title = "", string msg = "")
    {
        this.title = title; 
        this.msg = msg;
        string command = "./scripts/push_window '" + title + "' '" + msg + "'";
        shell.Bash(command);
    }

    public void SongPlay()
    { 
        shell.Bash("./scripts/push_sound");	
    }

    public bool MailSend()
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("david.perov60@gmail.com"));
        email.To.Add(MailboxAddress.Parse("david.perov60@gmail.com"));
        email.Subject = title;
        email.Body = new TextPart(TextFormat.Html) { Text = "<h1>" + msg + "</h1>" };

        using var smtp = new SmtpClient();
        // smtp.gmail.com
        smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        smtp.Authenticate("david.perov60@gmail.com", "dmvgevwuyfbyfhzw");
        smtp.Send(email);
        smtp.Disconnect(true);

        return true;
    }
    
}