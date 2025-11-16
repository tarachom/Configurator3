# Конфігуратор 3
<b>Конструктор для моделювання програми для обліку</b> | .net 10, Linux, Windows <br/>
    
<hr />

Детальніше про програму [accounting.org.ua](https://accounting.org.ua/configurator.html)<br/>
Середовище розробки [Visual Studio Code](https://code.visualstudio.com)<br/>
База даних [PostgreSQL](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads)<br/>
Документація [GtkSharp](https://accounting.org.ua/watch/section/news/code-00000015) та [SQL](https://accounting.org.ua/watch/section/note/code-00000057)<br/>

<b>Для Ubuntu</b><br/>
Встановити dotnet-sdk-10.0

    sudo apt-get update && sudo apt-get install -y dotnet-sdk-10.0

Довідка як встановити dotnet-sdk-10.0 на Ubuntu [Install .NET SDK or .NET Runtime on Ubuntu](https://learn.microsoft.com/uk-ua/dotnet/core/install/linux-ubuntu-install?tabs=dotnet10&pivots=os-linux-ubuntu-2204)

Встановити PostgreSQL

    sudo apt-get update
    sudo apt-get install postgresql

    # Встановити пароль для PostgreSQL

    sudo -u postgres psql
    \password postgres

<b>Для Windows</b> 
1. Встановити набір бібліотек GTK [gtk3-runtime](https://accounting.org.ua/download/gtk3-runtime-3.24.31-2022-01-04-ts-win64.exe)
2. Встановити dotnet 10.0 SDK [Download .NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
3. Встановити базу даних [PostgreSQL](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads)
4. Скачати архів з програмою [Конфігуратор](https://accounting.org.ua/download/Configurator3.zip)