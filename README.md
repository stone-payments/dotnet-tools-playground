# Dotnet Tools Playground

Um repositório para aprender sobre as ferramentas de build e teste para ecossistema dotnet.  
Este repositório contém duas aplicações idênticas, uma com modelo de projeto novo suportado a partir do Visual Studio 2017 e outro com o modelo suportado até o Visual Studio 2015.

## Pré-requisitos

- S.O. Windows 8 ou maior.
- [Build Tools for Visual Studio 2017](https://www.visualstudio.com/pt-br/thank-you-downloading-visual-studio/?sku=BuildTools&rel=15)
- [Visual Studio Test Agent 2017](https://www.visualstudio.com/pt-br/thank-you-downloading-visual-studio/?sku=TestAgent&rel=15)
    - Apenas instalação de arquivos não necessita conectar a um controller.
- [Dotnet Framework 4.6.2 Developer Pack](https://www.microsoft.com/net/download/windows)
- [Dotnet Core SDK](https://www.microsoft.com/net/download/windows)
- [Nuget CLI](https://www.nuget.org/downloads)
- [OpenCover](https://github.com/OpenCover/opencover/releases)
- [ReportGenerator](https://github.com/danielpalme/ReportGenerator/releases)


## Anatomia projeto dotnet

Um repositório dotnet é composto de uma solução e um ou mais projetos.

![folder tree view](images/folder-tree-view.png)

* Solução - Arquivo com extensão .sln mantém relação de projetos que fazem parte da solução.

* Projetos - Arquivos com extensão .*proj geralmente .csproj (C#) ou .vbproj(Visual Basic). 
Os arquivos de projeto contém informações como framework alvo de compilação, depêndencias e plataforma alvo (x86, x64).
Uma solução geralmente é divida em projetos utilizando conceito de camadas, como core, acesso a dados e apresentação. Testes também são colocados em projetos a parte.

* Arquivos de configuração - Arquivos com extensão .config, são arquivos de configurações clássicos de aplicações dotnet, seguem formato xml e contém parametrizações das aplicações como connections strings.

* Dependencia de pacotes - Os pacotes nuget (gerenciador de pacotes dotnet) necessários para compilar o projeto são declarados no arquivo packages.config.

## Baixando dependencias com NuGet

NuGet é o gerenciador de pacotes para bibliotecas dotnet. A maioria dos projetos dependem de pacotes NuGet para serem compilados, por isso devemos restaurar essas dependencias antes de compilar o código fonte.

O comando `nuget restore` pode receber como parâmetro o arquivo .sln ou .csproj. Se receber um .sln ele instala as dependencias listadas nos packages.config de cada projeto. Se receber um .csproj, baixa somente as dependencias desse projeto listadas no packages.config.

Após fazer do download do nuget.exe execute o seguinte comando:
``` powershell
nuget.exe restore .\src\BankAccountCliVS2015\BankAccountCliVS2015.sln
```
Os pacotes serão baixados na pasta packages no diretório do arquivo .sln.  
![nuget restore](images/nuget-restore.png)

## Compilando com o MSBuild

Para compilar projetos dotnet utilizamos o auxiliar de compilação e executor de tarefas **MSBuild**.

O utilitário de linha de comando *msbuild.exe* recebe como parâmetro um arquivo de solução(.sln) ou projeto (.*proj) e gera um ou mais *assemblies* dotnet em formato **.dll** ou **.exe**.

### Configurando MSBuild

Para localizar o executavel do msbuild você deve executar o seguinte comando:

``` powershell
reg.exe query "HKLM\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0" /v MSBuildToolsPath
```

Você pode adicionar esse diretório ao PATH com o comando abaixo para facilitar a utilização os comandos do msbuild. Deve executado em um terminal powershell como administrador.

``` powershell
[Environment]::SetEnvironmentVariable("PATH", $env:PATH + "<saída do comando anterior>", [System.EnvironmentVariableTarget]::User)
```

### Compilando projetos da solução

Vamos utilizar o MSBuild para compilar a solução BankAccounCliVS2015 que contém o modelo de csproj antigo encontrado na maioria dos projetos **dotnet framework**.

Execute o seguinte comando para compilar a solução que contém dois projetos:

``` powershell
msbuild .\src\BankAccountCliVS2015\BankAccountCliVS2015.sln
```

A saída deve ser algo como abaixo:
![msbuild output](images/msbuild-output.png)

Por padrão o projeto é compilado em modo Debug, o diretório padrão de saída é `bin\Debug` na pasta do projeto (onde é localizado o .csproj).  
Diretório de saída:  
![msbuild output files](images/msbuild-output-files.png)

### Debug x Release

O MSBuild por padrão compila os projetos em modo Debug, oque quer dizer que gera artefatos preparados para debug através da IDE, além de não estarem otimizados para uso em produção.  
Já em modo Release o compilador gera artefatos otimizados para produção.

Execute o seguinte comando para compilar em modo Release:
``` powershell
msbuild .\src\BankAccountCliVS2015\BankAccountCliVS2015.sln /p:Configuration=Release
```

O diretório padrão de saída é `bin\Release`.

### Outros usos do MSBuild

Além de ser utilizado para compilar os projetos dotnet o MSBuild também é um automatizador de tarefas que podem executar rotinas pré e pós build, adicionando *Tasks* no xml do projeto (.csproj).

Alguns exemplos de tasks são:
- Gerar artefato no formato de deploy para IIS.
- Empacotar aplicação com chocolatey
- Validação de scheme de um xml.

Vamos criar uma task simples para entender como funciona, edite o arquivo BankAccountCli.csproj e adicione as tasks ao fim do arquivo:

```xml
...
  <Target Name="BeforeBuild">
    <Exec Command="powershell -c Get-Date > $(OutputPath)\build_duration.txt"/>
  </Target>
  <Target Name="AfterBuild">
    <Exec Command="powershell -c Get-Date >> $(OutputPath)\build_duration.txt"/>
  </Target>
</Project>
```

Essas duas tasks irão escrever no arquivo build_duration.txt no diretório de saída a data/hora de ínicio e fim do build.
Execute o build novamente e veja o resultado.

## Testando com vstest

O vstest é um executor de testes para projetos dotnet utilizado pelo Visual Studio. Ele serve tanto para executar projetos com xunit, mstest e outros, abstraindo a interface com cada ferramenta.

O vstest.console.exe fica localizado na pasta de instalação do visual studio em `C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow`. 

Para executar os testes, o projeto de testes deve ser primeiro compilado, o que já foi executado na etapa anterior, pois quando passamos uma solução como parâmetro para o MSBuild todos os projetos são compilados.  
Em seguida devemos invocar o vstest.console.exe passando como parâmetro a dll de output do projeto de teste.

Execute o seguinte comando powershell para rodar os testes:

``` powershell
& 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe' .\src\BankAccountCliVS2015\BankAccountCli.UnitTest\bin\Debug\BankAccountCli.UnitTest.dll
```

Saída do comando:  
![vstest output](images/vstest-output.png)

## Cobertura de testes com OpenCover

O OpenCover é uma ferramenta de cobertura de testes open source para o ambiente dotnet. 

O `openconver.console.exe` fica localizado na pasta `C:\Users\<SEU-USUARIO>\AppData\Local\Apps\OpenCover\OpenCover.Console.exe` , mas para a sua facilidade você pode incluí-lo em seu PATH.

O output do OpenCover é um arquivo `.xml` pouco legível que contém os resultados do coverage, mas que podemos parsear com a ferramenta ReportGenerator, que será abordada mais para frente.

Para executar o OpenCover é necessário que você indique qual é o comando que executa os testes, incluindo os argumentos necessários, utilizando os parâmetros `-target:<comando de testes>` e `-targetargs:<parâmetros do comando de testes>`. Como utilizamos o `vstest`, o `-target:` é o path dele, e o `-targetargs:` são os argumentos que ele consume.

O OpenCover acaba fazendo a análise de coverage da própria DLL do projeto de testes, o que não faz muito sentido, por isso é recomendado utlizarmos um filtro para ignorar as classes de testes e considerar apenas o código fonte. Este filtro pode ser passado como o parâmetro `-filter:`, aonde podemos seguir a sintaxe do filtro para incluir todo o projeto e excluir qualquer assembly terminado em `UnitTest`, da seguinte forma : `-filter:"+[*]* -[*UnitTest]*"`. 

Um outro parâmetro interessante é o `-skipautoprops`, que faz com que o OpenCover não faça a cobertura de propriedades auto implementadas, como getters e setters.

Podemos especificar o caminho e nome do `output.xml` com o parâmetro `-output:`. 

Por último, também é preciso especificar o parâmetro `-register:`, que diz se o binário a ser testado é de 32 (`-register:path32`) ou 64 bits(`-register:path64`).

Execute o seguinte comando powershell para gerar o report de coverage:
```powershell
 C:\Users\<SEU-USUARIO>\AppData\Local\Apps\OpenCover\OpenCover.Console.exe -target:"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" -targetargs:"src\BankAccountCliVS2015\BankAccountCli.UnitTest\bin\Debug\BankAccountCli.UnitTest.dll" -register:path32 -skipautoprops -filter:"+[*]* -[*UnitTest]*"
```

A saída do comando deve ser como abaixo:

![open cover output](images/opencover-output.png)

E o arquivo `output.xml` será gerado na pasta em que o comando foi gerado ou no local especificado pelo parametro `-output:`.

## Melhorando o output do OpenCover com o ReportGenerator

Como a saída do OpenCover é um xml com baixíssima legibilidade, é comum utilizarmos a ferramenta ReportGenerator para ler este `.xml` e gerar um HTML com melhor visualização.

Uma vez baixado o `ReportGenerator.exe`, podemos indicar o caminho do XML fonte com o parâmetro `-reports:`, e do diretório de saída do HTML e das imagens com o `-targetdir:`.

Também podemos especificar quais tipos de report queremos com o `-reporttypes:` , que incluem em diversas opções como gerar em `HTML`, `Latex` ou outros. O que costumamos utilizar é o report `Html`, que é a opção padrão e gera um HTML com um bom overview e links para os reports mais detalhados de cada classe.

Execute o seguinte comando powershell para gerar o HTML com o coverage:

```
 C:\Users\<SEU-USUARIO>\tools\ReportGenerator_3.1.2.0\ReportGenerator.exe -reports:.\output.xml -targetdir:Report -reporttypes:Html
```

Dentro da pasta `Report` você encontrará um `index.html` com o coverage com uma boa visualização.

![report generator output](images/report-generator-output.png)


