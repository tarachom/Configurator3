# Конфігуратор 3

<img src="https://accounting.org.ua/images/configuration.png?v=3" /> <b>Програма для проектування бази даних PostgreSQL</b> | .net 7, Linux, Windows <br/>
    
 <b>Можливості</b>
    
    Проектування бази даних та генерування коду
    Вигрузка та загрузка даних або конфігурації, обслуговування бази даних

<img src="https://accounting.org.ua/images/configurator1.png" />

<hr />

 <b>Встановлення dotnet-sdk для Ubuntu 22.10</b>
 
 Детальніше - [Install the .NET SDK or the .NET Runtime on Ubuntu](https://learn.microsoft.com/uk-ua/dotnet/core/install/linux-ubuntu)<br/>
 
    wget https://packages.microsoft.com/config/ubuntu/22.10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    
    sudo apt-get update && sudo apt-get install -y dotnet-sdk-7.0

 <b>Встановлення PostgreSQL для Ubuntu</b>
 
 Детальніше - [PostgreSQL](https://www.postgresql.org/download/linux/ubuntu/)<br/>
 
    # Create the file repository configuration:
    sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'
    
    # Import the repository signing key:
    wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo apt-key add -
    
    # Update the package lists:
    sudo apt-get update
    
    # Install the latest version of PostgreSQL.
    # If you want a specific version, use 'postgresql-12' or similar instead of 'postgresql':
    sudo apt-get -y install postgresql

    # Встановлення пароля для postgres
    sudo -u postgres psql
    password postgres

 <b>Збірка</b>
    
    git clone https://github.com/tarachom/Configurator3.git
    git clone https://github.com/tarachom/AccountingSoftwareLib.git
    
    dotnet build Configurator3 --output Configurator3/bin/Debug/net7.0
    
    mkdir -p bin
    cp -r Configurator3/bin/Debug/net7.0/* bin

<hr />
 
Детальніше про програму [accounting.org.ua](https://accounting.org.ua/configurator.html)<br/>
Середовище розробки [Visual Studio Code](https://code.visualstudio.com)<br/>
База даних [PostgreSQL](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads)<br/>
