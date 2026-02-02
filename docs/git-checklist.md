\# Git checklist



\## Старт задачи

1\. git checkout main

2\. git pull

3\. git checkout -b feature/<name>



\## Во время работы

\- маленькие коммиты

\- перед коммитом: git status

\- добавляю файлы аккуратно (лучше git add -p)



\## Перед PR

\- dotnet build

\- dotnet test

\- git log --oneline --decorate -10 (проверить историю)



\## PR

\- заголовок: что сделано

\- описание: что/зачем/как проверить

\- после merge удаляю ветку на GitHub



