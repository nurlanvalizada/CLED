namespace CledAcademy.Core.Models.StaticFiles
{
    public class EmailTemplates
    {
        public static EmailTemplate BonusForPayment = new EmailTemplate
        {
            Title = "CLED ACADEMY - BONUS",
            SubTitle = "HÖRMƏTLİ İSTİFADƏÇİ! KOMPANİYAMIZIN İŞTİRAKÇISI OLDUĞUNUZ ÜÇÜN SİZƏ TƏŞƏKKÜR EDİRİK.",
            Body =
                "Kompaniyamız çərçivəsində siz son ödədiyiniz <b>{0} AZN</b> məbləğ qarşılığında <b>{1} AZN</b> balansınıza bonus " +
                "qazandınız. Təbrik edirik! Daha çox ödəniş edin və daha çox bonuslar qazanın. " +
                "<br/><br/><b>Kompaniyalarımız haqqında daha ətraflı məlumat əldə etmək üçün saytımızın xəbərlər bölməsini mütəmadi olaraq izləyin</b> <br/> " +
                "<a href=\"http://cledacademy.com/Home/News/\" title=\"CLEDAcademy Xəbərlər\">CLEDAcademy Xəbərlər</a>"
        };

        public static EmailTemplate Welcome = new EmailTemplate
        {
            Title = "CLED ACADEMY - UĞURLU QEYDİYYAT HAQQINDA",
            SubTitle =
                "TƏBRİK EDİRİK. SİZİN QEYDİYYATINIZ UĞURLA TAMAMLANMIŞDIR. ARTIQ SİZ HESABINIZA DAXİL OLMAQLA SAYTIMIZIN BÜTÜN XİDMƏTLƏRİNDƏN İSTİFADƏ EDƏ BİLƏRSİNİZ",
            Body =
                "CLEDAcademy həm öyrənmə, həm öyrətmə fəaliyyətləri ilə məşğul olan və tələbələrin yeni bilik, bacarıq və yeni nailiyyətlər əldə etmək məqsədilə qoşulduğu onlayn tədris platformasıdır. " +
                "CLEDAcademy tərəfindən təklif olunan dərslər yüksək səviyyəli ekspert müəllimlər tərəfindən tədris edilir. Burada hər bir dərs istəyə uyğun olaraq seçilə bilir və yalnız ehtiyac olan bölmələrin üzərində işləməyə daha çox imkan yaranır. " +
                "<br/><br/><b>Daha ətraflı məlumat əldə etmək üçün aşağıdakı keçidlərdən istifadə edin ...</b> <br/> " +
                "<a href=\"http://cledacademy.com/Courses/FreeVideos/\" title=\"Pulsuz Video Dərslər\">Pulsuz Video Dərslər</a><br />" +
                "<a href=\"http://cledacademy.com/Courses/\" title=\"Haqqımızda\">Mövcud Kurslar</a><br />" +
                "<a href=\"http://cledacademy.com/Home/About/\" title=\"Haqqımızda\">Haqqımızda</a><br />" +
                "<a href=\"http://cledacademy.com/Home/Contact/\" title=\"Bizimlə Əlaqə\">Bizimlə Əlaqə</a><br /> <br />" +
                "<b>Qeyd: </b>Siz dostlarınızı saytımıza dəvət etməklə bonuslar qazana bilərsiniz. Belə ki, dəvət etdiyiniz hər bir istifadəçi qeydiyyatdan keçdikcə siz 1 AZN bonus qazanacaqsınız. Bunun üçün aşağıdakı linki dostunuza göndərin.<br/> " +
                "<a href='{0}' title='CLEDAcademy Qeydiyyat'>{0}</a> <br/>"
        };

        public static EmailTemplate BonusForFriendRegistration = new EmailTemplate
        {
            Title = "CLED ACADEMY - BONUS",
            SubTitle = "HÖRMƏTLİ İSTİFADƏÇİ! SAYTIMIZI DOSTLARINIZA TÖVSİYYƏ ETDİYİNİZ ÜÇÜN TƏŞƏKKÜR EDİRİK.",
            Body =
                "Si̇zi̇n tövsi̇yyəni̇z əsasinda <b>{0} {1}</b> adlı i̇sti̇fadəçi̇ saytımızda qeydi̇yyatdan keçmi̇şdi̇r. " +
                "Yeni̇ kompani̇yamiza əsasən si̇z balansınıza 1 AZN bonus qazandiniz. Təbri̇k edi̇ri̇k! " +
                "<br/><br/><b>Balansınızı yoxlamaq üçün hesabınıza daxil olaraq aşağıdakı keçiddən istifadə edə bilərsiniz</b> <br/> " +
                "<a href=\"http://cledacademy.com/Account/\" title=\"Mənim Hesabım\">Mənim Hesabım</a>"
        };

        public static EmailTemplate SuggestToFriend = new EmailTemplate
        {
            Title = "CLED ACADEMY - DƏVƏT",
            SubTitle = "HÖRMƏTLİ İSTİFADƏÇİ! <b>{0} {1}</b> ADLI İSTİFADƏÇİ SİZİ SAYTIMIZDA QEYDİYYATDAN KEÇMƏYƏ DƏVƏT EDİR.",
            Body =
                "<b>Qeydiyyatdan keçmək üçün aşağıdakı linkdən istifadə edə bilərsiniz</b> <br/> " +
                "<a href='{0}' title='CLEDAcademy Qeydiyyat'>{0}</a> <br/>" +
                "Biz Azərbaycanda onlayn təhsilin inkişafı ilə məşğul olan platformayıq. Siz də saytımızda qeydiyyatdan keçə bilər və mövcud üstünlüklərdən və kampaniyalardan " +
                "faydalana bilərsiniz. Siz dəvət etdiyiniz hər bir dostunuza görə balansınıza 1 AZN bonus qazanacaqsınız." +
                "<br/><br/><b>Daha ətraflı məlumat əldə etmək üçün aşağıdakı keçidlərdən istifadə edin ...</b> <br/> " +
                "<a href=\"http://cledacademy.com/Courses/FreeVideos/\" title=\"Pulsuz Video Dərslər\">Pulsuz Video Dərslər</a><br />" +
                "<a href=\"http://cledacademy.com/Courses/\" title=\"Haqqımızda\">Mövcud Kurslar</a><br />" +
                "<a href=\"http://cledacademy.com/Home/About/\" title=\"Haqqımızda\">Haqqımızda</a><br />" +
                "<a href=\"http://cledacademy.com/Home/Contact/\" title=\"Bizimlə Əlaqə\">Bizimlə Əlaqə</a><br />"
        };

        public static EmailTemplate ForgotPassword = new EmailTemplate
        {
            Title = "CLED ACADEMY - UNUDULMUŞ ŞİFRƏNİN BƏRPASI",
            SubTitle = "SİZ ŞİFRƏNİZİN BƏRPASI ÜÇÜN MÜRACİƏT ETMİSİNİZ. YENİ ŞİFRƏ YARATMAQ ÜÇÜN AŞAĞIDAKI KEÇİDDƏN İSTİFADƏ EDİN",
            Body = "<a href='{0}' title='Şifrənizi bərpa edin'>{0}</a>"
        };
    }
}