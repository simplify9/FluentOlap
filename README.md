# FluentOlap

[![Build Status](https://dev.azure.com/simplify9/Github%20Pipelines/_apis/build/status/simplify9.FluentOlap?branchName=master)]
| **Package**       | **Version** |
| :----------------:|:----------------------:|
| ``SimplyWorks.FluentOlap``|![Nuget](https://img.shields.io/nuget/v/SimplyWorks.FluentOlap?style=for-the-badge)|

[SW.FluentOlap](https://www.nuget.org/packages/SimplyWorks.FluentOlap/) is an analysis tool that
expands and denormalizes models, along with adding the ability to link external models into its
denormalized form. It also adds the ability to populate this denormalized model by accessing
different APIs the user registers. 

It uses the de facto fluent syntax found in [EntityFramework](https://github.com/dotnet/efcore) and
[FluentValidation](https://github.com/FluentValidation/FluentValidation) so it should be very
familiar and easy to use.

## Getting Started 

Download the nuget package [SW.FluentOlap](https://www.nuget.org/packages/SimplyWorks.FluentOlap/)
and then define analytical models by defining classes that inherit from `AnalyticalObject<T>` where
`T` is the type of entity to be analyzed. In the contructor of said class, begin using the fluent
syntax like `Property(entity => entity.property)`

To populate, simply use the `PopulateAsync` method on an instance of the "Analyzer" class.

## External Dependencies

- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)


## Getting support 👷
If you encounter any bugs, don't hesitate to submit an
[issue](https://github.com/simplify9/FluentOlap/issues). We'll get back to you promptly!


