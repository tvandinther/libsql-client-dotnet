default:
    @just --choose

generate-bindings *OPTIONS:
    ./generate-bindings.sh {{OPTIONS}}

test:
    dotnet test

test-file *FILE:
    dotnet test --filter "FullyQualifiedName~Libsql.Client.Tests.{{FILE}}" 
