mogate
======

Mogate is basic maze game prototype developed for testing different approaches for entity system framework. The game part itself is based on MonoGame framework and currently works under Windows OS. The project files are tested and developed in Xamarin Studio.

Entity framework
----------------

Everything in the game is an entity but the entity doesn't use traditional objects oriented hierarchy. Instead of class inheritance programming model the enity can contain one or more behaviours. For example:

- Position - defines entity position in game world
- Drawable - contains information required to render entity
- Attackable - used to attack entity by others
- Attack - entity uses the attack to take a damage to other attackable entities

## Actions ##

While behaviours are used to describe entity's attributes an action is used to provide a logic on top of behaviours.

Since an entity contains only behaviours the action is also controlled by a behavior. It is called *Execute*.

This behavior contains queue of *IAction* objects and calls *Execute* function provided by *IAction* interface. Once *Execute* returns true the action is treated as completed and next one is dequeued for update.

Actions itself are divided into two groups:

1. Generic purpose actions
2. Gameplay actions

Generic purpose actions mostly designed to organize flow for other actions. For example:

- Sequence - provide queue for set of actions
- Spawn - allow to run one or more actions at the same time
- Loop - run other action forever
- Action - once run a function

Gameplay actions are focused on game aspects:

- MoveSpriteTo - used to move sprite from *Drawable* behavior to new position
- AnimSprite - enable animation
- FollowSprite - link one sprite to another
- AttackEntity - implement gameplay attack logic

Using gameplay actions outside of gameplay services allow to identify and isolate gameplay aspects and reuse it in the same way for different entities.

For example we want to move entity to new position with animation:

	var seq = new Sequence ();
	var spawn = new Spawn ();
	spawn.Add (new MoveSpriteTo (Player, new Vector2 (newPos.X * 32, newPos.Y * 32), 300));
	spawn.Add (new AnimSprite (Player, "move", 300));
	seq.Add (spawn);
	seq.Add (new ActionEntity (Player, (_) => {
		Player.Get<Position> ().MapPos = newPos;
	}));
	seq.Add (new ActionEntity (Player, OnEndMove));

	Player.Get<Execute> ().AddNew (seq, "movement");
	Player.Get<State<HeroState>> ().EState = HeroState.Moving;

In example below we use sequence to guarantee order of actions execution and spawn to syncronize animation with sprite movement. Also we use action to change map and state behaviours once visual part of movement flow will be finished.









