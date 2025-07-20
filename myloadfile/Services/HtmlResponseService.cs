using myloadfile.Models;

namespace myloadfile.Services;

// HTML Responses
public static class HtmlResponseService
{
    public static string ResponsePreOpenerLink(FormContext ctx)
    {
        if (string.IsNullOrWhiteSpace(ctx.TYPELISTCART)) return "";
        string cleanedType = ctx.TYPELISTCART.Replace("\r\n", "");
        string cleanedRemark = ctx.REMARKFILE.Replace("\r\n", "");
        return $@"
    // Добавляем новый лист осмотра (в зависимости от существования кнопок - с талоном или нет)
    try {{
        var i = window.opener.document.getElementById(""newrecnocase"");
        if (i == null) i = window.opener.document.getElementById(""newrec"");
        if (i != null) {{ i.click(); }}
        else alert(""No find button newrec"");
    }} catch(e) {{ alert(e); }}

    // Добавляем ссылку в подвал страницы
    let user = window.opener.document.getElementById(""USER"").value;
    let nameFileListCart = window.opener.document.getElementById(""nameFileListCart"").value.split(""_"");
    let dataListCart = nameFileListCart[1] + ""/"" + nameFileListCart[2] + ""/"" + nameFileListCart[3];
    let timeListCart = nameFileListCart[4] + "":"" + nameFileListCart[5] + "":"" + nameFileListCart[6].substring(0, nameFileListCart[6].length - 4);
    let TITLESPECLISTCARTOPENERLINK = user + "" {cleanedRemark} "" + dataListCart + "" "" + timeListCart;

    let div = window.opener.document.getElementsByClassName(""TITLESPECLISTCART"");
    let divItog = [];
    for (var title of div)
        if (
            title.textContent.includes(""Рентген/маммография"") ||
            title.textContent.includes(""ФЛГ"") ||
            title.textContent.includes(""УЗИ"") ||
            title.textContent.includes(""ЭКГ"") ||
            title.textContent.includes(""Рецепты"") ||
            title.textContent.includes(""Внешние документы"")
        )
            divItog.push(title);

    for (var button of divItog) {{
        if (button.textContent == ""{cleanedType}"") {{
            button.innerHTML += `<button type='button' class='TITLESPECLISTCART'
                style='text-align:center;width:100%;border:none;cursor:pointer;background-color:white'
                data-button-speclistcart-2
                onclick=""var i=getEl('slc1');if (i.style.display=='none') {{i.style.display='';getEl('slc1').scrollIntoView(false)}} else {{i.style.display='none'}}"">
                ${'{'}TITLESPECLISTCARTOPENERLINK{'}'}
            </button>`;
        }}
    }}
    ";
    }
    public static string ResponseSuccessHtml(FormContext ctx)
    {
        return
$@"Content-type: text/html

<HTML>
<HEAD>
    <meta http-equiv='CONTENT-TYPE' content='text/html' charset='windows-1251'>
</HEAD>
<BODY>
    <div class='TITLE' style='text-align:center'>Прием файла на сервере. Вер.:3.0</div>
    <HR>Принято байт: {ctx.ContentLength}<BR>
    <HR>Пользователь: {ctx.USER}<BR>
    <HR>Тип листа осмотра: {ctx.TYPELISTCART}<BR>
    <HR>Статус листа осмотра: {ctx.STATUSLISTCART}<BR>
    <HR>Файлы успешно загружены на сервер!<BR>
    <HR>Имя файла для сетевого доступа: {ctx.NameFileInternet}<BR>
</BODY>
</HTML>
<script>
    window.opener.document.getElementById('NAMEIMAGEFILE').value = '{ctx.NameFileInternet}';
    window.opener.document.getElementById('REMARKLISTCART').value = '{ctx.REMARKFILE.Replace("\r\n", "")}';
    window.opener.document.getElementById('TYPELISTCART').value = '{ctx.TYPELISTCART.Replace("\r\n", "")}';
    window.opener.document.getElementById('STATUSNEWLISTCART').value = '{ctx.STATUSLISTCART.Replace("\r\n", "")}';
    {ctx.PREOPENERLINK}
    window.opener.document.querySelector('[data-list-cart]').innerHTML += `{ctx.OPENERLINK}`;
    setTimeout('window.close()', 5000);
</script>";
    }
    public static string ResponseErrorHtml(Exception errorMessage)
    {
        return
$@"Content-type: text/html

<HTML>
<HEAD>
    <meta http-equiv='CONTENT-TYPE' content='text/html' charset='windows-1251'>
</HEAD>
<BODY>
    <div class='TITLE' style='text-align:center'>Прием файла на сервере. Вер.:3.0</div>
    <HR>УПС<BR>
    <HR>Во время обработки данных формы возникла ошибка: {errorMessage.Message}<BR>
</BODY>
</HTML>";
    }
}