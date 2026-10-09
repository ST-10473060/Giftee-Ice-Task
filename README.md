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
