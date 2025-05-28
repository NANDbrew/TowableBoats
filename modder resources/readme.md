## shipmaker's guide

If you want to put cleats on your ships they require the following:
- each cleat must have a GPButtonDockMooring component  (and its requirements, look at existing dock moorings for reference)
- each cleat must be tagged `Boat`
- cleats must all be children of a transform called `towing set` in your boat's main transform
- container GameObject can be inactive if it's not a shipyard option, it will get enabled when it's detected

You can also import the unity package from the modder resources folder, which contains a prefab with the correct setup.