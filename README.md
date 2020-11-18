# Tp.Core.Functional

Tp.Core.Functional contains implementations of Maybe monad, Try monad and Either type.

## Note for Github users

Primary source-of-truth repository is hosted on internal GitLab. Github repository is a read-only mirror.

## Maybe monad

Maybe monad is an implementation of [Option type](http://en.wikipedia.org/wiki/Option_type). It represents encapsulation of an optional value; e.g. it is used as the return type of functions which may or may not return a meaningful value when they are applied.

Usages:

```csharp
var i = Maybe.Just(1); // Create a value
var m = i.Select(x=>x*2); // map value, result is Just(2)
var f = m.Where(x=>x%2==1); // filter value, result is Nothing
var n = m.Bind(x=>Maybe.Just(2*x)) // flatMap (or bind) value, result is Just(4)
```

Tp.Functional.Core does not require to specify a type of `Maybe.Nothing`:

```csharp
Maybe<int> n = Maybe.Nothing;
Maybe<int> m = Maybe<int>.Nothing;
n == m; // True
```

See more [here](https://github.com/TargetProcess/Tp.Core.Functional/wiki/Maybe)

## Either type

Either type represents disjoint union of two values:

```csharp
var left = Either.CreateLeft<int,string>(1);
var right = Either.CreateRight<int,string>("string");
```
See more [here](https://github.com/TargetProcess/Tp.Core.Functional/wiki/Either)

## Try monad

A Try wraps a computation that could either result in a value or in an exception being thrown:

```csharp
var tryResult = Try.Create(()=>int.Parse(value));

var m = try.Select(x=>x*2);
```

See more [here](https://github.com/TargetProcess/Tp.Core.Functional/wiki/Try)










