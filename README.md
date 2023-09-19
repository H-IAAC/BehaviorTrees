# Behavior Trees

Unity implementation of basic behavior trees and extensions such as utility and external trees.

It also provides a visual interface for editing the behavior trees.

This project was developed as part of the Cognitive Architectures research line from 
the Hub for Artificial Intelligence and Cognitive Architectures (H.IAAC) of the State University of Campinas (UNICAMP).
See more projects from the group [here](https://github.com/brgsil/RepoOrganizer).

[![](https://img.shields.io/badge/-H.IAAC-eb901a?style=for-the-badge&labelColor=black)](https://hiaac.unicamp.br/)
[![](https://img.shields.io/badge/-Arq.Cog-black?style=for-the-badge&labelColor=white&logo=data:image/svg%2bxml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4gPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSI1Ni4wMDQiIGhlaWdodD0iNTYiIHZpZXdCb3g9IjAgMCA1Ni4wMDQgNTYiPjxwYXRoIGlkPSJhcnFjb2ctMiIgZD0iTTk1NS43NzQsMjc0LjJhNi41Nyw2LjU3LDAsMCwxLTYuNTItNmwtLjA5MS0xLjE0NS04LjEtMi41LS42ODksMS4xMjNhNi41NCw2LjU0LDAsMCwxLTExLjEzNi4wMjEsNi41Niw2LjU2LDAsMCwxLDEuMzY4LTguNDQxbC44LS42NjUtMi4xNS05LjQ5MS0xLjIxNy0uMTJhNi42NTUsNi42NTUsMCwwLDEtMi41OS0uODIyLDYuNTI4LDYuNTI4LDAsMCwxLTIuNDQzLTguOSw2LjU1Niw2LjU1NiwwLDAsMSw1LjctMy4zLDYuNDU2LDYuNDU2LDAsMCwxLDIuNDU4LjQ4M2wxLC40MSw2Ljg2Ny02LjM2Ni0uNDg4LTEuMTA3YTYuNTMsNi41MywwLDAsMSw1Ljk3OC05LjE3Niw2LjU3NSw2LjU3NSwwLDAsMSw2LjUxOCw2LjAxNmwuMDkyLDEuMTQ1LDguMDg3LDIuNS42ODktMS4xMjJhNi41MzUsNi41MzUsMCwxLDEsOS4yODksOC43ODZsLS45NDcuNjUyLDIuMDk1LDkuMjE4LDEuMzQzLjAxM2E2LjUwNyw2LjUwNywwLDAsMSw1LjYwOSw5LjcyMSw2LjU2MSw2LjU2MSwwLDAsMS01LjcsMy4zMWgwYTYuNCw2LjQsMCwwLDEtMi45ODctLjczMmwtMS4wNjEtLjU1LTYuNjgsNi4xOTIuNjM0LDEuMTU5YTYuNTM1LDYuNTM1LDAsMCwxLTUuNzI1LDkuNjkxWm0wLTExLjQ2MWE0Ljk1LDQuOTUsMCwxLDAsNC45NTIsNC45NUE0Ljk1Nyw0Ljk1NywwLDAsMCw5NTUuNzc0LDI2Mi43MzlaTTkzNC44LDI1Ny4zMjVhNC45NTIsNC45NTIsMCwxLDAsNC4yMjEsMi4zNDVBNC45Myw0LjkzLDAsMCwwLDkzNC44LDI1Ny4zMjVabS0uMDIyLTEuNThhNi41MTQsNi41MTQsMCwwLDEsNi41NDksNi4xTDk0MS40LDI2M2w4LjA2MSwyLjUuNjg0LTEuMTQ1YTYuNTkxLDYuNTkxLDAsMCwxLDUuNjI0LTMuMjA2LDYuNDQ4LDYuNDQ4LDAsMCwxLDIuODQ0LjY1bDEuMDQ5LjUxOSw2LjczNC02LjI1MS0uNTkzLTEuMTQ1YTYuNTI1LDYuNTI1LDAsMCwxLC4xMTUtNi4yMjksNi42MTgsNi42MTgsMCwwLDEsMS45NjYtMi4xMzRsLjk0NC0uNjUyLTIuMDkzLTkuMjIyLTEuMzM2LS4wMThhNi41MjEsNi41MjEsMCwwLDEtNi40MjktNi4xbC0uMDc3LTEuMTY1LTguMDc0LTIuNS0uNjg0LDEuMTQ4YTYuNTM0LDYuNTM0LDAsMCwxLTguOTY2LDIuMjY0bC0xLjA5MS0uNjUyLTYuNjE3LDYuMTMxLjc1MSwxLjE5MmE2LjUxOCw2LjUxOCwwLDAsMS0yLjMsOS4xNjRsLTEuMS42MTksMi4wNiw5LjA4NywxLjQ1MS0uMUM5MzQuNDc1LDI1NS43NSw5MzQuNjI2LDI1NS43NDQsOTM0Ljc3OSwyNTUuNzQ0Wm0zNi44NDQtOC43NjJhNC45NzcsNC45NzcsMCwwLDAtNC4zMTYsMi41LDQuODg5LDQuODg5LDAsMCwwLS40NjQsMy43NjIsNC45NDgsNC45NDgsMCwxLDAsNC43NzktNi4yNjZaTTkyOC43LDIzNS41MzNhNC45NzksNC45NzksMCwwLDAtNC4zMTcsMi41LDQuOTQ4LDQuOTQ4LDAsMCwwLDQuMjkxLDcuMzkxLDQuOTc1LDQuOTc1LDAsMCwwLDQuMzE2LTIuNSw0Ljg4Miw0Ljg4MiwwLDAsMCwuNDY0LTMuNzYxLDQuOTQsNC45NCwwLDAsMC00Ljc1NC0zLjYzWm0zNi43NzYtMTAuMzQ2YTQuOTUsNC45NSwwLDEsMCw0LjIyMiwyLjM0NUE0LjkyMyw0LjkyMywwLDAsMCw5NjUuNDc5LDIyNS4xODdabS0yMC45NTItNS40MTVhNC45NTEsNC45NTEsMCwxLDAsNC45NTEsNC45NTFBNC45NTcsNC45NTcsMCwwLDAsOTQ0LjUyNywyMTkuNzcyWiIgdHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTkyMi4xNDMgLTIxOC4yKSIgZmlsbD0iIzgzMDNmZiI+PC9wYXRoPjwvc3ZnPiA=)](https://github.com/brgsil/RepoOrganizer)

## Repository Structure

- BehaviorTrees: Unity project with the package


## Installation / Usage


### Installation

- Open the `Package Manager`
- Click the add **+** button in the status bar, select "`Add Package from git URL...`"
- Enter the url: `https://github.com/H-IAAC/BehaviorTrees.git?path=BehaviorTrees`

### Usage

- Create a new `Behavior Tree` in "Create\Behavior Tree\Behavior Tree"
- Double click for open the editor.
- Add the nodes and connect to the root.
- Assign the tree to run on a object with the `Behavior Tree Runner` component.


## Authors

- (2022-) [EltonCN](https://github.com/EltonCN): undergraduated, IC-Unicamp
  
## References

Basic implementation:

[1] M. Dawe, S. Gargolinski, L. Dicken, T. Humphreys, and D. Mark, “Behavior Selection Algorithms,” in Game AI Pro 360 (S. Rabin, ed.), pp. 1–14, CRC Press, 1 ed., Sept. 2019

[2] Why Memory Nodes is a Bad Idea in Behavior Trees (intro to BTs part 5B), (Jan. 07, 2022). Accessed: Sep. 14, 2023. [Online Video]. Available: https://www.youtube.com/watch?v=W7p34qhBux8

[3] Unity | Create Behaviour Trees using UI Builder, GraphView, and Scriptable Objects [AI #11], (May 28, 2021). Accessed: Sep. 14, 2023. [Online Video]. Available: https://www.youtube.com/watch?v=nKpM98I7PeM

[4] Behaviour Tree Editor with UI Builder - Part2 [AI #12], (Jun. 18, 2021). Accessed: Sep. 14, 2023. [Online Video]. Available: https://www.youtube.com/watch?v=jhB_GFgS6S0

[5] UNITY DIALOGUE GRAPH TUTORIAL - Variables and Search Window, (Mar. 18, 2020). Accessed: Sep. 14, 2023. [Online Video]. Available: https://www.youtube.com/watch?v=F4cTWOxMjMY

Decision Behavior Trees:

[5] B. Merrill, “Building Utility Decisions into Your Existing Behavior Tree”.

[6] Utility Behavior Trees (intro to BTs part 12), (Nov. 16, 2021). Accessed: Sep. 14, 2023. [Online Video]. Available: https://www.youtube.com/watch?v=ju4GY1gtP8s

External Trees and Behavior tags:

[7] Martin Cerny, Tomas Plch, Matej Marko, Petr Ondracek, and Cyril Brom, “Smart Areas - A Modular Approach to Simulation of Daily Life in an Open World Video Game:,” in Proceedings of the 6th International Conference on Agents and Artificial Intelligence, (ESEO, Angers, Loire Valley, France), pp. 703–708, SCITEPRESS - Science and and Technology Publications, 2014.

[8] Free Range AI: Creating Compelling Characters for Open World Games, (Mar. 12, 2017). Accessed: Sep. 14, 2023. [Online Video]. Available: https://www.youtube.com/watch?v=jDCFMITrtHc


## Acknowledgements

This project is part of the Hub for Artificial Intelligence and Cognitive Architectures
(H.IAAC- Hub de Inteligência Artificial e Arquiteturas Cognitivas). We acknowledge the 
support of PPI-Softex/MCTI by grant 01245.013778/2020-21 through the Brazilian Federal Government.