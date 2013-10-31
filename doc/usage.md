## Usage of SharpDoc command line

The command line options of `SharpDoc` are:

    Usage: SharpDoc [options]* [--config file.xml | Assembly1.dll Assembly1.xml...]*
    Documentation generator for .Net languages
    
    options:
     -c, --config=VALUE     Configuration file
     -D=VALUE1:VALUE2       Define a template parameter with an (optional)
      value.
     -S=VALUE1:VALUE2       Define a style parameter with a (optional) value.
     -d, --style-dir=VALUE  Add a style directory
     -s, --style=VALUE      Specify the style to use [default: Standard]
     -o, --output=VALUE     Specify the output directory [default: Output]
     -r, --references=VALUE Add reference assemblies in order to load source
      assemblies
     -w, --webdoc=VALUE1:VALUE2 Url of the extern documentation site [with the
      protocol to use, ex: http(s)://...]
     --wL, --webdocLogin=VALUE1:VALUE2 (optional) Authentification data for the extern
      documentation site [userName:password]
    
     -h, --help Show this message and exit
    
    [Assembly1.dll Assembly1.xml...] Source files, if a config file is not
    specified, load source assembly and xml from the specified list of files
    
    Styles available:
       Name: Standard, Path: C:\Code\Paradox\externals\SharpDoc\build\Styles\Standard
       Name: Mshc, Path: C:\Code\Paradox\externals\SharpDoc\build\Styles\Mshc, Base: Standard
    

## Example usage

### Generate a documentation for an assembly

Simply run the command:

	SharpDoc YourAssembly.dll YourAssembly.xml

and It will generate the documentation under the sub-folder `Output`.

You can pass several assemblies and xml files at once. The condition is that an Assembly (`dll` or `exe`) should be associated with its generated documentation file `xml`. 

### Generate a documentation with a TOC and several assemblies

TODO Document how to use the config files

[Test link to root documentation](main.md)

This is a test link to the namespace [SharpDoc](N:SharpDoc)

Test to external link [SharpDoc on GitHub](https://github.com/xoofx/SharpDoc)

