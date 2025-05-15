# Mutation Testing Report – .NET Application

## Overview

This report evaluates the effectiveness of various test suites (unit, integration, and security) through **mutation testing**. The objective was to measure how well each test suite detects intentional code mutations introduced by [**Stryker.NET**](https://stryker-mutator.io/), a mutation testing tool for .NET projects.

Mutation testing assesses **test quality** by introducing small code changes (mutants) and verifying if existing tests can detect them (kill them). A high percentage of *killed* mutants indicates robust test coverage and strong assertion quality.

## Test Categories and Results

| Test Type         | Mutants Generated | Mutants Killed | Kill Rate | Comments |
|-------------------|-------------------|----------------|-----------|----------|
| Unit Tests        | High              | ~90%           | ✅ 90%    | Strong effectiveness in catching code mutations. |
| Integration Tests | High              | ~90%           | ✅ 90%    | Solid assertion quality and code interaction coverage. |
| Security Tests    | 70                | 26             | ⚠️ 37.1%  | Not designed for mutation detection; lower kill rate expected. |

Security tests are typically scenario-based, focusing on threat modeling and protection layers rather than mutation kill accuracy. The lower kill rate reflects their misalignment with mutation testing goals, not poor quality.

## Screenshot

A detailed snapshot of the mutation test run is available below, showcasing the mutation testing summary in the terminal:

![Mutation Testing Screenshot](https://github.com/ionutdd/TestareaSistemelorSoftware/blob/main/BacklogTests/Mutations1.png)

## Key Takeaways

- **Unit and Integration Tests** demonstrated high effectiveness, achieving a **90% kill rate**, indicating strong validation of code correctness.
- **Security Tests** yielded a lower kill rate (**37.1%**), consistent with their focus on non-functional testing, such as system resilience and vulnerability resistance, rather than unit-level logic flaws.
- Mutation testing complements code coverage metrics by revealing whether tests genuinely verify behavior, beyond merely executing code lines.
