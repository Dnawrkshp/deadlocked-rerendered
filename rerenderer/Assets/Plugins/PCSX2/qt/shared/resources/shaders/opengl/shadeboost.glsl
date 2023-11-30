/*  PCSX2 - PS2 Emulator for PCs
 *  Copyright (C) 2002-2023 PCSX2 Dev Team
 *
 *  PCSX2 is free software: you can redistribute it and/or modify it under the terms
 *  of the GNU Lesser General Public License as published by the Free Software Found-
 *  ation, either version 3 of the License, or (at your option) any later version.
 *
 *  PCSX2 is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 *  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 *  PURPOSE.  See the GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along with PCSX2.
 *  If not, see <http://www.gnu.org/licenses/>.
 */

//#version 420 // Keep it for editor detection

/*
** Contrast, saturation, brightness
** Code of this function is from TGM's shader pack
** http://irrlicht.sourceforge.net/phpBB2/viewtopic.php?t=21057
** TGM's author comment about the license (included in the previous link)
** "do with it, what you want! its total free!
** (but would be nice, if you say that you used my shaders  :wink: ) but not necessary"
*/

#ifdef FRAGMENT_SHADER

uniform vec4 params;

in vec4 PSin_p;
in vec2 PSin_t;
in vec4 PSin_c;

layout(binding = 0) uniform sampler2D TextureSampler;

layout(location = 0) out vec4 SV_Target0;

// For all settings: 1.0 = 100% 0.5=50% 1.5 = 150%
vec4 ContrastSaturationBrightness(vec4 color)
{
	float brt = params.x;
	float con = params.y;
	float sat = params.z;

	// Increase or decrease these values to adjust r, g and b color channels separately
	const float AvgLumR = 0.5;
	const float AvgLumG = 0.5;
	const float AvgLumB = 0.5;

	const vec3 LumCoeff = vec3(0.2125, 0.7154, 0.0721);

	vec3 AvgLumin = vec3(AvgLumR, AvgLumG, AvgLumB);
	vec3 brtColor = color.rgb * brt;
	float dot_intensity = dot(brtColor, LumCoeff);
	vec3 intensity = vec3(dot_intensity, dot_intensity, dot_intensity);
	vec3 satColor = mix(intensity, brtColor, sat);
	vec3 conColor = mix(AvgLumin, satColor, con);

	color.rgb = conColor;
	return color;
}


void ps_main()
{
	vec4 c = texture(TextureSampler, PSin_t);
	SV_Target0 = ContrastSaturationBrightness(c);
}


#endif
