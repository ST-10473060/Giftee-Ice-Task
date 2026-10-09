# 🎁 Giftee

> The right gift, without the guesswork.

Giftee is a gift recommendation web app. You tell it about a friend (their age, your budget, their personality, their interests, and their favourite artist and TV show) and it returns five ranked gift ideas, each with a reason. Built for the CLDV6212 ICE task.

*Live site:* https://giftee-web.lemonmeadow-497ebd9e.southafricanorth.azurecontainerapps.io

> Both services scale to zero when idle, so the first request after a quiet period can take up to a minute while they wake up.

## Group members

| Name | Student number |
|---|---|
| Pavankumar Naidu | [Student number] |
| Desun Loganathan | ST10473408 |
| Mohammed Luay Saib | [Student number] |
| Devesh Naidu | [Student number] |
| Khashif Ahmed Dawood | [Student number] |
| Nabiha Osman | [Student number] |
| Ismaeel Kajee | [Student number] |
| Tyron James Seamark | [Student number] |

## Contents

- [The problem](#the-problem)
- [Target users](#target-users)
- [Features](#features)
- [Why music and TV](#why-music-and-tv)
- [How the scoring works](#how-the-scoring-works)
- [Tech stack](#tech-stack)
- [Architecture](#architecture)
- [Run it locally](#run-it-locally)
- [Environment variables](#environment-variables)
- [Tests](#tests)
- [CI/CD](#cicd)
- [Deployment](#deployment)
- [Data sources and AI use](#data-sources-and-ai-use)
- [Limitations](#limitations)
- [References](#references)
  
## The problem

Buying a gift is stressful. You want something thoughtful, within your budget, and you often have little time. Surveys point to the same problems:

- In a 2024 survey of 2,000 UK adults, 58% had received at least one Christmas gift they didn't want, rising to 80% of Gen Z. The average unwanted gift was worth about £41 (Finder, 2026).
- A Gumtree survey reported in 2016 found that nearly 60% of South Africans had received an unwanted gift (ITWeb, 2016).
- A 2014 Rakuten survey across seven markets found that 46% of Americans named not knowing what to buy as a main stress factor (Retail Merchandiser, 2014).

These are commercial surveys about Christmas, so we treat them as indicators, not proof. When people don't know what to buy, they grab something generic at the last minute, waste money on gifts that go unused, or lose hours scrolling.

## Target users

Giftee is for anyone buying a gift for a friend or family member, especially students and busy people on a budget, who know what someone likes but not what to buy.

## Features

- A simple form: age, budget (in rand), personality, interests, favourite artist and favourite TV show.
- Five ranked gift ideas, each with a price range, description, score and the reasons it was picked.
- Hard filters for age and budget, so unsuitable or age-restricted gifts never appear.
- A fallback to popular all-rounders when nothing matches, so the results are never empty.
- 92 gifts in the catalogue, with matching built in for 100 TV shows and 200 artists (100 singers and 100 rappers).
- A homepage that explains the app, with a Start button.

## Why music and TV

We chose music and TV because they are the taste signals you usually already know about a friend. Research links music taste to personality: Rentfrow and Gosling's study of over 3,500 people found that music preferences relate to personality (Rentfrow and Gosling, 2003; KaraFun, 2026). Popular writing also suggests that the shows we rewatch reflect what we value (Cotler, 2026). We treat this as a clue, not a rule, so artist and show sit alongside interests, personality, age and budget in the scoring.

## How the scoring works

Scoring is in backend/Giftee.Api/Services/ScoringService.cs. It has two steps.

*1. Filters.* A gift is removed if the person's age is outside its age range, or if its minimum price is above the budget.

*2. Points.*

| Match | Points |
|---|---|
| An interest matches a gift's tags or category | +3 |
| An interest only appears in the gift's description | +1 |
| The favourite artist or show matches a gift's tags or name | +6 |
| The gift is a general music or TV gift (no direct match) | +1 |
| The personality type matches the gift | +2 |

The top five gifts are returned, sorted by score and then alphabetically. Each result lists its reasons. If nothing matches, the first five eligible gifts are returned as "popular all-rounders"


## Tech stack

| Layer | Technology |
|---|---|
| Frontend | ASP.NET Core MVC (.NET 10), Razor views, Bootstrap and custom CSS |
| Backend | ASP.NET Core Web API (.NET 10), Swagger (Swashbuckle) |
| Data | PostgreSQL through Entity Framework Core (Npgsql). Postgres 16 container locally, Neon in production |
| Containers | Docker and Docker Compose |
| CI/CD | GitHub Actions, CodeQL, Dependabot, GitHub Container Registry |
| Hosting | Azure Container Apps (Consumption plan) and Neon (free tier) |

## Architecture


Browser  →  Giftee.Web (ASP.NET Core MVC)  →  Giftee.Api (ASP.NET Core Web API)  →  PostgreSQL


- Giftee.Web renders the pages and calls the API over HTTP. The API address comes from API_BASE_URL.
- Giftee.Api holds the scoring logic and the data. On first start it creates the table and loads the 92 gifts from gifts.json.
- The same code runs locally (Docker Compose) and in the cloud. Only the environment variables change.

*API endpoints*

| Method | Path | Description |
|---|---|---|
| GET | /api/health | Health check |
| GET | /api/gifts | All gifts, ordered by name |
| POST | /api/recommendations | Ranked recommendations. Body: age, budget, personality, favouriteArtist, favouriteShow, interests |

/api/recommendations requires an age between 1 and 120, a budget of zero or more, and at least one interest, artist, show or personality type.

*Project structure*


.
├── .github/
│   ├── workflows/ci.yml        # CI/CD pipeline
│   └── dependabot.yml          # weekly dependency updates
├── backend/
│   ├── Giftee.Api/             # Web API
│   │   ├── Data/               # DbContext, seeder, gifts.json
│   │   ├── Models/
│   │   ├── Services/           # ScoringService
│   │   ├── Program.cs
│   │   └── Dockerfile
│   └── Giftee.Tests/           # unit tests
├── frontend/
│   └── Giftee.Web/             # MVC web app
│       ├── Controllers/
│       ├── Models/
│       ├── Services/           # ApiClient
│       ├── Views/
│       ├── wwwroot/
│       └── Dockerfile
├── docker-compose.yml
├── .env.example
├── .dockerignore
├── Giftee.slnx
└── README.md


## Run it locally

*You need:* Git and Docker Desktop (with the engine running).

1. Clone the repository and open the folder in a terminal.
2. Create your .env file from the example.

   Windows PowerShell:
powershell
   Copy-Item .env.example .env

   macOS or Linux:
bash
   cp .env.example .env

3. Start everything:
bash
   docker compose up --build

4. Open *http://localhost:5200* in your browser. The API is at http://localhost:5100 (try /api/gifts).
5. Stop it with docker compose down. Add -v to also delete the database volume.

The first start creates the table and loads the gifts. The seeder only runs when the table is empty, so if you change gifts.json, run docker compose down -v and start again.

Use Docker Compose to run the whole app. Starting one project from Visual Studio on its own has no database to connect to.
