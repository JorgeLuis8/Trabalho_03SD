# Thermal Monitoring System (SensorClient)

Este projeto consiste em um sistema distribuído de monitoramento de temperatura seguindo a arquitetura de três camadas. O sistema simula a leitura de sensores de hardware, processa esses dados em um servidor centralizado e persiste as informações em um banco de dados relacional, garantindo a idempotência das operações.

---

## Arquitetura do Sistema

O projeto foi dividido em três camadas distintas para garantir a separação de responsabilidades:

**1. Camada de Apresentação (Client)**

- Desenvolvida em .NET MAUI (C#).
- Interface moderna em Dark Mode com feedback visual em tempo real.
- Responsável pela geração de UUIDs únicos para cada requisição e simulação de dados térmicos.

**2. Camada de Aplicação (Server)**

- Desenvolvida com FastAPI (Python).
- Implementa as regras de negócio (classificação de temperatura em Normal, Alerta ou Crítico).
- Filtro de idempotência: Verifica se o UUID já foi processado antes de persistir, evitando duplicidade.

**3. Camada de Dados (Persistence)**

- Utiliza SQLite com SQLAlchemy (ORM).
- Armazenamento persistente das leituras, IDs de sensores e timestamps.

---

## Tecnologias Utilizadas

**Front-end**

- .NET 9.0 / MAUI
- HttpClient (Consumo de API REST)
- ObservableCollection (Atualização dinâmica de UI)

**Back-end**

- Python 3.11+
- FastAPI & Uvicorn
- SQLAlchemy (ORM)
- SQLite

---

## Pré-requisitos

### Instalando o .NET 9.0 e o Workload do MAUI

**1. Baixe e instale o .NET 9.0 SDK**

Acesse o site oficial e baixe o instalador para o seu sistema operacional: https://dotnet.microsoft.com/download/dotnet/9.0


Após a instalação, verifique se está correto executando:
```bash
dotnet --version
```

O retorno deve ser `9.0.x`.

**2. Instale o workload do MAUI**

Com o .NET instalado, execute o comando abaixo no terminal para instalar o workload necessário:
```bash
dotnet workload install maui
```

**3. Verifique a instalação**

Para confirmar que o MAUI foi instalado corretamente:
```bash
dotnet workload list
```

O workload `maui` deve aparecer na lista.

> **Observacao:** O desenvolvimento com .NET MAUI para Windows requer o Visual Studio 2022 ou superior com o componente "Desenvolvimento para plataformas moveis com .NET" habilitado. Caso prefira usar o Visual Studio, baixe em `https://visualstudio.microsoft.com/`.

---

## Como Executar o Projeto

### 1. Configurando o Servidor (Back-end)

Navegue até a pasta do servidor e instale as dependências:
```bash
pip install fastapi uvicorn sqlalchemy
```

Inicie o servidor:
```bash
python main.py
```

O servidor estará rodando em `http://localhost:5000`. Você pode acessar a documentação automática em `http://localhost:5000/docs`.

### 2. Configurando o Cliente (Front-end)

Na pasta do cliente, execute:
```bash
dotnet run -f net9.0-windows10.0.19041.0
```

---

## Regras de Negócio

O servidor classifica as temperaturas recebidas conforme a seguinte lógica:

| Temperatura | Status | Descrição |
|---|---|---|
| `< 0°C` ou `> 35°C` | CRITICO | Condições extremas de operação. |
| `0°C a 10°C` ou `28°C a 35°C` | ALERTA | Faixas de atenção (resfriamento/aquecimento). |
| `11°C a 27°C` | NORMAL | Faixa de operação ideal. |

---

## Idempotência

Cada leitura enviada pelo cliente contém um UUID v4. O servidor armazena esse ID. Caso o cliente reenvie a mesma leitura (devido a uma falha de rede ou clique duplo), o servidor identifica o ID duplicado no banco de dados, ignora a nova gravação e retorna o status já processado, mantendo a integridade dos dados.

---



## Autores

Jorge Luis Ferreira Luz  
Alisson Rodrigo Carneiro da Silva