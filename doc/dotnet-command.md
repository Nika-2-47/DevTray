# dotnet コマンドライン

## ソリューションの操作
- 空のソリューションを作成する
  - 　`dotnet new sln -n MySolution`

- プロジェクトをソリューションに追加する
  - 　`dotnet sln MySolution.sln add MyProject/MyProject.csproj`

## プロジェクトの作成
-  コンソールアプリケーションを作成する
   -  `dotnet new console -n MyConsoleApp`
-  クラスライブラリを作成する
   -  `dotnet new classlib -n MyClassLib`