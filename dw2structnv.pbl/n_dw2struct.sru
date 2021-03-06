$PBExportHeader$n_dw2struct.sru
namespace
namespace dw2structnv
end namespace

forward
global type n_dw2struct from NonVisualObject
end type
end forward

global type n_dw2struct from NonVisualObject
end type
global n_dw2struct n_dw2struct

forward prototypes
public function string GenerateStructure (string syntax, string structname)
public function string GenerateNonvisual (string syntax, string nvname)
end prototypes

public function string GenerateStructure (string syntax, string structname);integer			index, count, rc
datastore		tempDs
string			errors
string			name, datatype
string			struct

tempDs = create datastore
rc = tempDs.create ( syntax, ref errors )

count = integer ( tempDs.Object.DataWindow.Column.Count )
for index = 1 to count
	
	name = tempDs.describe ( '#' + string ( index ) + '.Name' )
	datatype = tempDs.describe ( '#' + String ( index ) + '.ColType' )
	choose case left( datatype, 4 )
		case "deci"
			datatype = left ( datatype, 7 )
		case "date", "long", "numb", "real", "ulong"
			// no change needed
		case "char"
			datatype = "string"
		case "int"
			datatype = "long"
		case "time"
			choose case datatype
				case "time"
					// no change needed
				case "timestamp"
					datatype = "datetime"
				case else
					// Don't recognize the datatype, skip the column
					continue
			end choose
		case else
			// Don't recognize the datatype, skip the column
			continue
	end choose
	
	struct += '~t' + datatype + '~t~t' + name + '~r~n'
	
NEXT

struct += 'end type'

struct = 'global type ' + structname + ' from structure~r~n' + struct

return struct
end function

public function string GenerateNonvisual (string syntax, string nvname);integer			index, count, rc
datastore		tempDs
string			errors
string			name, datatype
string			nvsource

tempDs = create datastore
rc = tempDs.create ( syntax, ref errors )

nvsource = "public:~r~n~r~n"

count = integer ( tempDs.Object.DataWindow.Column.Count )
for index = 1 to count
	
	name = tempDs.describe ( '#' + string ( index ) + '.Name' )
	datatype = tempDs.describe ( '#' + String ( index ) + '.ColType' )
	choose case left( datatype, 4 )
		case "deci"
			datatype = left ( datatype, 7 )
		case "date", "long", "numb", "real", "ulong"
			// no change needed
		case "char"
			datatype = "string"
		case "int"
			datatype = "long"
		case "time"
			choose case datatype
				case "time"
					// no change needed
				case "timestamp"
					datatype = "datetime"
				case else
					// Don't recognize the datatype, skip the column
					continue
			end choose
		case else
			// Don't recognize the datatype, skip the column
			continue
	end choose
	
	nvsource += datatype + '~t~t' + name + '~r~n'
	
NEXT

nvsource += "end variables~r~n" + &
"~r~n" + &
"on " + nvname + ".create~r~n" + &
"call super::create~r~n" + &
"TriggerEvent( this, ~"constructor~" )~r~n" + &
"end on~r~n" + &
"~r~n" + &
"on " + nvname + ".destroy~r~n" + &
"TriggerEvent( this, ~"destructor~" )~r~n" + &
"call super::destroy~r~n" + &
"end on~r~n"

nvsource = "$PBExportHeader$" + nvname + ".sru~r~n" + &
"forward~r~n" + &
"global type " + nvname + " from NonVisualObject~r~n" + &
"end type~r~n" + &
"end forward~r~n" + &
"~r~n" + &
"global type " + nvname + " from NonVisualObject~r~n" + &
"end type~r~n" + &
"global " + nvname + " " + nvname + "~r~n" + &
"~r~n" + &
"type variables~r~n" + nvsource

return nvsource
end function

on n_dw2struct.create
call super::create
TriggerEvent( this, "constructor" )
end on

on n_dw2struct.destroy
TriggerEvent( this, "destructor" )
call super::destroy
end on
