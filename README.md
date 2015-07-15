# XMT-Scalper
Traduction du robot XMT Scalper en CSharp à partir de son source en Mql4.

# Origine de XMT-Scalper

'XMT-Scalper' est un robot c'est à dire un programme informatique ou Expert Advisor (EA) fonctionnant sur la plateforme de trading MetaTrader 4
Il est originellement issu d'une version préliminaire  d'un robot commercial nommé 'MillionDollarPips' mais il a été largement renforcée et amélioré.
Aujourd'hui, tout le code a été réécrit à partir de zéro, et seulement une partie du cœur de la stratégie est identique à sa version de base. 
Cet EA n'est pas un produit commercial, mais est disponible gratuitement.

# Objectifs

Le Projet initial  d'un robot de trading scalper XMT-Scalper v2.4.6.1 écrit en MQL4 provient de [Capella](http://www.worldwide-invest.org). 
Il à été traduit en CSharp par [Abdallah Hacid](ab.hacid@gmail.com) avec l'aide de la librairie [NQuotes](http://www.nquotes.net/) de [Daniel](support2@nquotes.net ).
A partir de là nous nous proposons de restructurer le code pour le rendre plus modulaire et ainsi plus lisible puis ensuite le faire évoluer plus 
simplement.

# Avantages
Un avantage indéniable du système NQuotes est que l'on peut écrire les indicateurs et robots de trading en CSharp et les éxécuter directemet
sur la plateforme MetaTrader 4, on peut également faire un débogage ou piloter le robot depuis une interface Windows Form ou WPF. 

# Installation

Pour compiler le programme avec Visual Studio, il faut au préalable :

1) installer [metatrader 4](http://www.metatrader4.com/).

2) installer [NQuotes](http://www.nquotes.net/installation)  

3) Modifier les instructions post-buids des deux projets MqlApiWithStdLib et XMT-Scalper
  
  remplacez dans 'xcopy "$(ProjectDir)$(OutDir)$(TargetFileName)" "C:\Users\{user}\AppData\Roaming\MetaQuotes\Terminal\{clé}\MQL4\Experts" /Y'

  a) votre propre clé unique d'installation de metatrader 4 qui peut être retrouvée dans le répertoire '%TERMINAL_DATA_PATH%' qui se trouve dans
  C:\Users\{user}\AppData\Roaming\MetaQuotes\Terminal\. 
  
  b) votre nom d'utilisateur windows.

 # Débogage avec NQuotes

  Pour déboguer le programme avec NQuotes suivez les [instructions](http://www.nquotes.net/expert-creation-tutorial) donnez par son auteur [Daniel](support2@nquotes.net ).

# Notes
  
  a) si metatrader est actif, il y a une erreur lors de la compilation (la copie des dll ne se fait pas) du fait que les dll sont bloquées en accès par metratrader.
  il faut donc fermer metatrader avant de compiler.

  
# Auteur
Je suis Abdallah Hacid, mon métier est [technicien informatique](http://www.dpaninfor.ovh) et j'habite dans l'Essonne en France.
