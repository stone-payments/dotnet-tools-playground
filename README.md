# Dotnet Tools Playground

## Anatomia projeto dotnet

Um repositório dotnet é composto de uma solução e um ou mais projetos.

![folder tree view](images/folder-tree-view.png)

* Solução - arquivo com extensão .sln mantém relação de projetos que fazem parte da solução.

* Projetos - Arquivos com extensão .*proj geralmente .csproj (C#) ou .vbproj(Visual Basic). 
Os arquivos de projeto contém informações como framework alvo de compilação, depêndencias e plataforma alvo (x86, x64).
Uma solução geralmente é divida em projetos utilizando conceito de camadas, como core, acesso a dados e apresentação. Testes também são colocados em projetos a parte.

* Arquivos de configuração - Arquivos com extensão .config, são arquivos de configurações clássicos de aplicações dotnet, seguem formato xml e contém parametrizações das aplicações como connections strings.

* Dependencia de pacotes - Os pacotes nuget (gerenciador de pacotes dotnet) necessários para compilar o projeto são declarados no arquivo packages.config.

## Compilando projetos da solução

Para compilar projetos dotnet utilizamos o auxiliar de compilação e executor de tarefas **MSBuild**.

O utilitário de linha de comando *msbuild.exe* recebe como parâmetro um arquivo de solução(.sln) ou projeto (.*proj) e gera um ou mais *assemblies* dotnet em formato **.dll** ou **.exe**.
