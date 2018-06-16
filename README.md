# Collections

For lack of a better name, I opted to rename the suite *"Collections"*, which includes ``ImmutableBitArray``, and the derivational work, ``Enumerations``.

## ImmutableBitArray

Initially I wanted to use the .NET Framework [System.Collections.BitArray](http://msdn.microsoft.com/en-us/library/system.collections.bitarray.aspx) for a couple of my applications, but soon discovered that it was neither [immutable](http://en.wikipedia.org/wiki/Immutable_object) nor [idempotent](http://en.wikipedia.org/wiki/Idempotence) under certain circumstances, especially for some key bitwise operations. Effectively, some operations that should return a new instance do not, which is incorrect behavior.

The operations are fairly self explanatory. The goals were clear getting stared: I wanted to establish a basic moral equivalence, so-called, but for the afore mentioned immutability and idempotency concerns. I will continue adding new operations, and will continue to flesh it out, or as issues and requests are submitted, or contributors want to add to the body of effort.

## Enumerations

I also wanted to support collections of [Java-like](http://docs.oracle.com/javase/7/docs/api/java/lang/Enum.html) ``Enumerations`` for .NET. Instead of [simple integral values](http://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/enum), I wanted to attach additional domain specific properties to each kind of Enumerated value. This is not supported in .NET, at least not directly, unlike Java, which supports class like behavior directly from enumerated values.

Why is this part of my *Collections* suite? Simple. Because, at its core, it depends on the ``ImmutableBitArray`` to support not only *Ordinal* operations, but especially for *Bitwise* operations.

### Enumeration Unit Testing

Instead of containing unit testing within the suite as a done deal, I opted to expose a robust set of unit tests for purposes of vetting ***your*** applications of ``Enumeration`` or ``Enumeration<T>``. This is key, because a lot can be told by the story of your own applications.

The testing framework is fairly robust and does depend upon [NUnit](http://nunit.org/) [2.6.4](http://www.nuget.org/packages/NUnit/2.6.4), at least for the time being. I may pursue a migration path, or add additional support, into [xunit](http://xunit.github.io/), for example, but this will depend heavily upon there being competent provisional [combinatorial](http://github.com/AArnott/Xunit.Combinatorial/) support. That, or firing the combinatorial author altogether and forge my own combinatorial path; however, bandwidth is the key contraint where these ambitions are concerned.

## Data Structures

As it turns out, there is not much work that is truly required to support Data Structure Patterns such as *Stacks*, *Queues*, and even one of my favorites, *Deques*, or *Double-ended Queues*. Additionally, the unit testing around these follows an extremely cohesive testing paradigm, which makes it that much easier to support.

## Future Goals

The next couple of things I want to pursue in this suite is to provide an Visual Studio extension for purposes of generating at least your boilerplate ``Enumeration`` details. I think such behavior should include the ability to specify whether the ``Enumeration`` is of an ``Ordinal`` or ``Bitwise`` variety, and, if bitwise, should also automatically generate things such as your bitwise operators. There are many ways I could approach this, I have not decided on the particulars just yet, but I welcome the feedback, posted issues, requests, etc.

As for *Data Structures*, I may look into first class actual collections, likely implemented in C++ CLI for performance reasons. However, for now, the syntactic sugar enhancing simple ``IList<T>`` and ``List<T>`` types are just fine for what I want to accomplish.

Thanks and enjoy!
