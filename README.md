# UCL-Simulator
Here is a complete, structured GitHub `README.md` file tailored for this project. It highlights the statistical complexity of the code while remaining accessible to other developers.

![UEFA Champions League](ucl.png)

---

# 🏆 UEFA Champions League Monte Carlo Simulator

A sophisticated C# console application that simulates the UEFA Champions League knockout stages using statistical modeling. Instead of relying on random number generation to arbitrarily pick winners, this engine calculates realistic match outcomes and player statistics using **Poisson Distributions** and **Monte Carlo Simulations**.

## 📖 About the Project

This project simulates the final stages of the Champions League (from the Quarter-Finals to the Final). It assigns Expected Goals (xG) based on each team's attacking strength and defensive vulnerabilities, simulates two-legged ties, calculates aggregate scores, handles penalty shootouts, and tracks individual player goals to crown the Golden Boot winner.

The initial dataset is pre-loaded with real-world context from the 2025–2026 UEFA Champions League season.

## ✨ Key Features

* **Monte Carlo Match Engine:** Simulates home-and-away knockout legs, accounting for home-field advantage.
* **Realistic Goal Generation:** Uses Donald Knuth's algorithm for calculating Poisson distributions to determine how many goals a team scores based on their specific matchup.
* **Golden Boot Tracker:** Distributes match goals to individual players using weighted probabilities based on their real-world attacking threat.
* **Dynamic Knockout Logic:** Automatically calculates aggregate scores, away/home balances, and forces penalty shootouts if a tie remains unbroken.

## 🧮 The Mathematics

Football is a low-scoring game, making it highly suitable for modeling via the **Poisson Distribution**. The engine calculates a team's probability of scoring $k$ goals in a match using the following formula:

$$P(k) = \frac{\lambda^k e^{-\lambda}}{k!}$$

* **$P(k)$**: The probability of scoring exactly $k$ goals.
* **$\lambda$ (Lambda)**: The team's Expected Goals for the match. This is calculated as: `(Team Attack Strength) × (Opponent Defense Weakness) × (Home Advantage Modifier)`.
* **$e$**: Euler's number.

## 🚀 Getting Started

### Prerequisites

* [.NET SDK](https://dotnet.microsoft.com/download) (6.0 or higher recommended)
* Visual Studio, VS Code, or any C# IDE.

### Installation & Execution

1. Clone the repository:
```bash
git clone https://github.com/yourusername/ucl-monte-carlo-simulator.git

```


2. Navigate to the project directory:
```bash
cd ucl-monte-carlo-simulator

```


3. Run the application:
```bash
dotnet run

```



## 📊 Team Configuration Structure

Teams and players are easily configurable in the `SetupTeams()` method. You can update the stats to simulate different tournaments or seasons.

| Property | Description | Example (Real Madrid) |
| --- | --- | --- |
| **Attack Strength** | Offensive capability modifier (higher is better). | 2.2 |
| **Defense Weakness** | Defensive vulnerability modifier (higher means conceding more). | 1.1 |
| **Scoring Weight** | Individual player's probability of claiming a team goal. | Mbappé (0.6), Vini Jr (0.3) |
| **Base Goals** | Pre-loaded goals from the Group Stages/Round of 16. | Mbappé (11) |

## 🛠️ Built With

* **C# / .NET** - Core application logic and object-oriented structure.
* **System.Linq** - For efficient querying, roster management, and sorting top scorers.
