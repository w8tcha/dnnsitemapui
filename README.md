# dnnsitemapui
WatchersNET.SiteMap - A Modern SiteMap / TreeView Module and Skin Object for DNN®

![prview](http://www.watchersnet.de/Portals/0/SiteMapSkinsCollage.png)

[DEMO](http://www.watchersnet.de/DotNetNuke/Module/SiteMap.aspx)

Back in the old days, almost every web site had a sitemap where they listed out all the pages. The purpose of the sitemap is to help visitors and search engine spiders to find information on the site.

Now, a lot of modern websites have dropped the sitemap page, instead they place the sitemap in the footer area. 

This Module/Skin Object Creates a SiteMap that looks like such footer SiteMap, but of course it can be placed anywhere on your Website. There a many pre-installed Skins, which changes the look of the SiteMap.

The Module/Skin Object also can Render the SiteMap as a TreeView (With Skin Support & Animations)

# Features

* SiteMap available as Module and Skin Object
* Very Light Weight
* Skin Support (Contains 9 Normal Skins, and 10 TreeView Skins)
* There are two Render Modes : Normal and as TreeView*
* Option to Show only Sub Links from Current Tab
* Simply excluding of Tabs
* Show / Hide Tab Hidden Tabs
* Show / Hide Tab Icons
* Define a Default Tab Icon
* Shows links based on visitor's authorized permission level
* Full Language Localization
* Generates Human Friendly URLs
* IPortable Support (Easy Import/Export of Settings)
* Option to Set Maximum Sublevels
* Demo Modus (Shows Skin Selector above the SiteMap)
* Filter Tabs By Taxonomy Tags (Terms from Current Page, All Terms from All Vacabularies, From Selected Vocabularies and from Selected Terms)

## Extra TreeView Options

* Animation: Sets the animation speed for Expanding/Collapsing of Child Tabs. 
* Valid values are one of the strings "slow", "normal", "fast".
* Collapse: Sets whether all nodes should be collapsed by default
* Unique: Sets whether only one tree node can be open at any time, collapsing any previous open nodes.
* Persist: Persists the tree's expand/collapse state in one of two ways: 
** "location" - looks within all tree nodes with link anchors that match the document's current URL (location.href), and if found, expands that node (including its parent nodes). Great for href-based state saving.
** "cookie" - Saves the current state of the tree on each click to a cookie and restores that state on page load.

## Option to Set Start Level

* Root : Shows All Tabs
* Parent : Shows All Tabs From the Current Tabs Level Parent - If No Parent Tabs available it Renders from Root
* Current: Shows All Tabs In the Current Level
* Children: Shows All Child Tabs From The Current Tab - If No Child Tabs available it Renders from Root)
* Custom: Shows all Tabs in The Current level based on the selected Tab
