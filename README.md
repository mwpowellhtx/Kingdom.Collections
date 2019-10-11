# .NET Conference 2018

Attendance for my talk was less than I had hoped, and I was not happy with the recorded performance furnished by the meeting vendor after all anyway. So I ended up recording a couple of debugging sessions and uploading them to my [YouTube](http://y2u.be/8V46CWH_a88) account. Enjoy, comment, like, subscribe, and if you find my services here or elsewhere of any value whatosever, please do not hesitate to contact me, and I would like very much to discuss terms of service in more detail.

# Collections

For lack of a better name, I opted to rename the suite *"Collections"*, which includes ``ImmutableBitArray``, and the derivational work, ``Enumerations``.

## Breaking Changes

### Bidirectionals Refactored

We refactored the `BidirectionalList` to the `Kingdom.Collections.Generic` namespace instead of `Kingdom.Collections` where it was before.

We also added a `BidirectionalDictionary` in addition to the `BidirectionalList`. This operates along similar lines as the list except that your *Add* and *Remove* callbacks accept both a *key* as well as the *value*.

## ImmutableBitArray

Initially I wanted to use the .NET Framework [System.Collections.BitArray](http://msdn.microsoft.com/en-us/library/system.collections.bitarray.aspx) for a couple of my applications, but soon discovered that it was neither [immutable](http://en.wikipedia.org/wiki/Immutable_object) nor [idempotent](http://en.wikipedia.org/wiki/Idempotence) under certain circumstances, especially for some key bitwise operations. Effectively, some operations that should return a new instance do not, which is incorrect behavior. I may rename the collection, and consequently the assembly, after all, to better reflect the *Idempotent* attribute that I found was the most critical; but for now, I am running with the name *Immutable*.

The operations are fairly self explanatory. The goals were clear getting started: I wanted to establish a basic moral equivalence, so-called, but for the afore mentioned immutability and idempotency concerns. I will continue adding new operations, and will continue to flesh it out, or as issues and requests are submitted, or contributors want to add to the body of effort.

I took a little time to improve performance by representing the *Immutable Bit Array* in terms of a collection of ``Byte``. This took a bit of effort, but I think the performance is about as strong as can be at present. Chiefly, there was also a trade off in terms of *Shift* capability involved in that there is no advantage spending the calories on figuring out the byte-wise shifts involved. Instead, I opted to simply treat the Shift in terms of a ``Boolean`` collection, which works out pretty well performance-wise.

Eventually, I may reconsider whether the application of the term *idempotent* is really that accurate. Upon further analysis, it seems to me the focus of whether something is idempotent has to do with the function itself not mutating the thing it is operating on, regardless of the outcome. And, while true, the *ones complement* operator, indeed *any* such operators, should leave the original *operand* untouched, this is not really the same thing, I think. I will need to study the issue a bit further to better name it, I think.

## Enumerations

I also wanted to support collections of [Java-like](http://docs.oracle.com/javase/7/docs/api/java/lang/Enum.html) ``Enumerations`` for .NET. Instead of [simple integral values](http://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/enum), I wanted to attach additional domain specific properties to each kind of Enumerated value. This is not supported in .NET, at least not directly, unlike Java, which supports class like behavior directly from enumerated values.

Why is this part of my *Collections* suite? Simple. Because, at its core, it depends on the ``ImmutableBitArray`` to support not only *Ordinal* operations, but especially for *Bitwise* operations.

### Enumeration Unit Testing

Instead of containing unit testing within the suite as a done deal, I opted to expose a robust set of unit tests for purposes of vetting ***your*** applications of ``Enumeration`` or ``Enumeration<T>``. This is key, because a lot can be told by the story of your own applications.

The testing framework is fairly robust and does depend upon [NUnit](http://nunit.org/) [2.6.4](http://www.nuget.org/packages/NUnit/2.6.4), at least for the time being. I may pursue a migration path, or add additional support, into [xunit](http://xunit.github.io/), for example, but this will depend heavily upon there being competent provisional [combinatorial](http://github.com/AArnott/Xunit.Combinatorial/) support. That, or firing the combinatorial author altogether and forge my own combinatorial path; however, bandwidth is the key contraint where these ambitions are concerned.

### FlagsEnumerationAttribute enabled Code Generation

My motivation here was to establish an seamless ``Enumeration<T>`` experience that looks and feels more or less like the language leven ``enum`` and ``FlagsAttribute``. As such, I wanted to enable automatic code generation of the boilerplate code that is necessary to override the bitwise operators with appropriate ``Enumeration<T>`` based counterparts.

I wanted to pursue this in terms of a *Visual Studio Extension* at first, but soon discovered that the better choice was to do so in terms of a *.NET Standard Analyzer and Code Fix*. As the name implies, this heavily depended upon first making the transition into *.NET Core/Standard*. As it turns out, this is doable, but not so simple on the surface, not least of all with respect to *Core/Standard* confusion throughout the industry today. Less so for me today, but the migration paths even within *Core/Standard* versions are still a bit muddy waters for me.

At the present time, there are a couple of aspects in the delivery. First, there is a *Code Fix* enabled by the *Analyzer* when the ``FlagsEnumerationAttribute`` is applied. This determines whether the target ``Enumeration<T> class`` is declared ``partial``, and provides a corresponding fix for when it has not.

The second is the code generation itself, around which the integration nuances are not fully resolved. Unit testing of which was also a primary motivation with the subsequent [*Code Analysis*](#net-code-analysis-code-fixes-and-other-fallout) section. Under the hood, code generation depends upon the [``CodeGeneration.Roslyn``](/AArnott/CodeGeneration.Roslyn) project, and ultimately upon command line ``code-gen`` bits.

At the time of this writing, *CodeGeneration.Roslyn* integration nuances were not fully working and are still to be determine. To be clear, and to be fair, I do not mean that *CodeGeneration.Roslyn* itself is not working; only in terms of my solution level comprehension of said bits. However, I am fairly confident that the ``FlagsEnumerationAttribute`` generator itself is working, and have commited the unit tests that prove this to be the case.

### .NET Code Analysis, Code Fixes, and other fallout

My pursuit of the *Analyzer and Code Fix* extensibility solution also led me to discover a couple of areas that deserved serious refactoring, thereby improving upon the boilerplate project template. Chiefly, [*Analyzer Diagnostics*](/mwpowellhtx/Kingdom.Collections/tree/master/src/Kingdom.CodeAnalysis.Verifiers.Diagnostics) and [*Code Fixes*](/mwpowellhtx/Kingdom.Collections/tree/master/src/Kingdom.CodeAnalysis.Verifiers.CodeFixes), their helpers, etc, deserve their own projects with separately delivered packages. This is especially true for code generation unit testing, which depends solely upon the analyzer diagnostics alone, wholely separate from the code fixes themselves.

In addition, there were [also a couple of extension methods](/mwpowellhtx/Kingdom.Collections/tree/master/src/Kingdom.CodeAnalysis.Verification) that I found helpful to more *fluently* verify that the code is properly generated. I intentionally steered clear of assuming any dependencies on [*xunit*](/xunit/xunit), or any other, test framework, at this level. Although, I left things fairly open to inject assertions via extension method *predicates*.

Ultimately, I will likely reposition these in a repository dedicated to this notion, but for the time being the projects live here at the point of discovery, at least until the immediate dust has settled a bit.

## Data Structures

As it turns out, there is not much work that is truly required to support Data Structure Patterns such as *Stacks*, *Queues*, and even one of my favorites, *Deques*, or *Double-ended Queues*. Additionally, the unit testing around these follows an extremely cohesive testing paradigm, which makes it that much easier to support.

## Future Goals

Re-writing any of these assemblies in terms of *C++ CLI* may be a non-starter at least in the near and medium term after all. From reading various blogs, etc, it seems as though it is not on the *Microsoft* agenda to migrate any *C++ CLI* support in terms of *.NET Core* or *.NET Standard* support.

I do still want to consider furnishing first class collection objects, not just syntactic sugar in the form of collection extension methods, but this effort is not high on my list of priorities at the moment.

Unit testing was a bit of a pickle this time around which proved to be a bit of a set back. I like to utilize *ReSharper* for my unit testing, but at present there are some issues with *ReSharper* *xUnit.net* settings being somehow misaligned. At present I do not know how this is happening, so my tests are being engaged via the *Visual Studio Test Explorer*, or via *console* when necessary.

Thank you so much and enjoy!
