
This script checks if a reference is in the Header/Function/Criteria descriptions it is also
a relationship to the referenced item. It will report any missing relationships.
For example:
	Criteria OV.1#01
	The system SHALL conform to function [[CP.9.1]] (Produce a Summary Record of Care).
	
Then there shall be a relationship from Function OV.1 to Function CP.9.1. 

This issue surfaced in the Child Health Developmental Screening Functional Profile and you
can recognize it by the following error message when generating the publication:

	[fop] WARNING: Page 21: Unresolved ID reference "CPS.6" found.

