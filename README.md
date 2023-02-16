# Конфігуратор 3

<img src="https://accounting.org.ua/images/configuration.png?v=3" /> <b>Програма для проектування бази даних PostgreSQL</b> | .net 7, Linux, Windows <br/>
    
 <b>Можливості</b>
    
    Легке проектування бази даних
    Структура конфігурації на українській мові
    Генерування коду на C#
    Вигрузка та загрузка даних або конфігурації
    Обслуговування бази даних

 <b>net 7.0 для Ubuntu 22.10</b>
 
 [Install the .NET SDK or the .NET Runtime on Ubuntu](https://learn.microsoft.com/uk-ua/dotnet/core/install/linux-ubuntu)<br/>
 
    wget https://packages.microsoft.com/config/ubuntu/22.10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    
    sudo apt-get update && sudo apt-get install -y dotnet-sdk-7.0

 <b>Збірка для Linux</b>
    
    git clone https://github.com/tarachom/Configurator3.git
    git clone https://github.com/tarachom/AccountingSoftwareLib.git
    
    dotnet build Configurator3 --output Configurator3/bin/Debug/net7.0
    
    mkdir -p bin
    cp -r Configurator3/bin/Debug/net7.0/* bin

<hr />
 
Детальніше про програму [accounting.org.ua](https://accounting.org.ua/configurator.html)<br/>
Середовище розробки [Visual Studio Code](https://code.visualstudio.com)<br/>
База даних [PostgreSQL](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads)<br/>
