///////////////////////////////////////////////////
//
// Fulldome lib
//
#ifndef SHLIB_CG_INCLUDED
#define SHLIB_CG_INCLUDED


///////////////////////////////////////////////////
//
// Misc
#define QUARTERPI		0.785398163397448
#define HALFPI			1.57079632679489661923
#define PI				3.14159265358979323846
#define TWOPI			6.28318530717958647692
#define toRadians(a)	((a)*0.017453292519943295769)
#define toDegrees(a)	((a)*57.295779513082321)

// Compare a vector to Zero
#define isNull3(v)	((v).x == 0 && (v).y == 0 && (v).z == 0 )

// debug stuff
float4 getColorRed()
{
	return float4(1,0,0,1);
}


///////////////////////////////////////////////////
//
// Fulldome stuff
//
// store 3D dome coordinate as texture
// Unit coordinates		(-1.0 .. 1.0)
// Texel coordinates	( 0.0 .. 1.0)
#define texelToUnit2(uv)		( ((uv) * 2.0) - float2(1,1) )
#define texelToUnit3(uv)		( ((uv) * 2.0) - float3(1,1,1) )
#define unitToTexel2(uv)		( ((uv) + float2(1,1)) * 0.5 )
#define unitToTexel3(uv)		( ((uv) + float3(1,1,1)) * 0.5 )

//
// pbourke bangalore.pdf pg 14
// http://paulbourke.net/miscellaneous/domefisheye/fisheye/
// From:	Dome 3D coordinates (-1.0 .. 1.0)
// To:		Texture coordinates (0.0 .. 1.0)
float2 domeToTexel( float3 pos, float horizon )
{
	float theta = atan2( sqrt( pos.x * pos.x + pos.y * pos.y ), pos.z);	// invert y/z ???
	float phi = atan2( pos.y, pos.x );
	float r = theta / (horizon * 0.5);
	float2 st = float2( r * cos(phi), r * sin(phi) );
	return unitToTexel2( st );
}
float2 domeToTexel( float3 pos )
{
	return domeToTexel( pos, PI );
}
float3 texelToDome( float2 st, float horizon )
{
	st = texelToUnit2( st );
	float r = sqrt( st.x * st.x + st.y * st.y );
	float theta = atan2( st.y, st.x );
	float phi = r * (horizon * 0.5);
	float3 pos;
	pos.x = sin(phi) * cos(theta);
	pos.y = cos(phi);
	pos.z = sin(phi) * sin(theta);
	float y = pos.y; pos.y = pos.z; pos.z = y;	// invert y/z ???
	return pos;
}
float3 texelToDome( float2 st )
{
	return texelToDome( st, PI );
}



//////////////////////////////////////////////
//
// CUBEMAPPING TO DOMEMASTER
//
#define FACE_NONE		0
#define FACE_FRONT		1	// front
#define FACE_RIGHT		2	// right
#define FACE_BACK		3	// back
#define FACE_LEFT		4	// left
#define FACE_TOP		5	// top
#define FACE_BOTTOM		6	// bottom
//
// Return cubemap coordinates to plot on a domemaster texture
//
//	in: texel (0.0 .. 1.0)
//	in: horizon level in degrees (default = 180.0)
//	out: z = face (#define'd above)
//	out: xy = coordinates
//
// http://stackoverflow.com/a/12292731/360930
float3 makeCubemapIndex ( float2 st, float horizon )
{
	// Outside Domemaster
	if ( length( texelToUnit2(st) ) > 1.0 )
		return float3(0,0,0);
	
	float3 dc = texelToDome( st, toRadians(horizon) );

	float ax = abs(dc.x);
	float ay = abs(dc.y);
	float az = abs(dc.z);

	float face;
	float2 coord;

	// X
	if ( ax >= ay && ax >= az )
	{
		// -X
		if (dc.x < 0)
		{
			face = float(FACE_LEFT);
			coord.x = dc.y / -dc.x;
			coord.y = dc.z / -dc.x;
		}
		// +X
		else
		{
			face = float(FACE_RIGHT);
			coord.x = -dc.y / dc.x;
			coord.y = dc.z / dc.x;
		}
	}
	// Y
	else if ( ay >= ax && ay >= az )
	{
		// -Y
		if (dc.y < 0)
		{
			face = float(FACE_BACK);
			coord.x = -dc.x / -dc.y;
			coord.y = dc.z / -dc.y;
		}
		// +Y
		else
		{
			face = float(FACE_FRONT);
			coord.x = dc.x / dc.y;
			coord.y = dc.z / dc.y;
		}
	}
	// Z
	else if ( az >= ax && az >= ay )
	{
		// -Z
		if (dc.z < 0)
		{
			face = float(FACE_BOTTOM);
			coord.x = dc.x / -dc.z;
			coord.y = dc.y / -dc.z;
		}
		// +Z
		else
		{
			face = float(FACE_TOP);
			coord.x = dc.x / dc.z;
			coord.y = -dc.y / dc.z;
		}
	}

	coord = unitToTexel2(coord);

	return float3(coord,face);
}











//
// Pack cubemap channel and X/Y pos inside RGBA
//
//	R			G			B			A
//	0000 0000	0000 0000	0000 0000	0000 0000
//	IIII IIII	IIII IIII	IIII IIII	AAAA AAAA
//
// I = Index = face * CUBEMAPSIZE ^ 2 + y * CUBEMAPSIZE + x
// A = Alpha, zero if outside Domemaster
//
//#extension GL_EXT_gpu_shader4 : enable	// enable bitwise operations
#define CUBEMAPSIZE		1672
#define CUBEMAPSIZESQ	2795584	// CUBEMAPSIZE^2
// Not using bitwise operators (for CG compatibility)
// http://nova-fusion.com/2011/03/21/simulate-bitwise-shift-operators-in-lua/
#define lshift(a,b)		int(       float(a) * pow(2.0, float(b)) )
#define rshift(a,b)		int(floor( float(a) / pow(2.0, float(b)) ))
#define btrim(a,b)		( int(a) % int(pow(2.0, float(b))) )	// btrim(a,4) = a & 0x0f, btrim(a,8) = a & 0xff
//	in: packed cubemap index (as above)
//	out: xy = coordinates on the face (0.0 .. 1.0)
//	out: z  = the cubemap face (int, #define'd above)
//	out: a  = alpha (copied from packed)
float4 unpackCubemapIndex_cg( float4 m )
{
	if ( m.a == 0.0 )
		return float4(0,0,0,0);
	int i = int(m.b*255.0) + lshift(int(m.g*255.0), 8) + lshift(int(m.r*255.0), 16) - 1;
	if ( i == 0 )
		return float4(0,0,0,0);
	int face = i / CUBEMAPSIZESQ + 1;
	int ii   = i % CUBEMAPSIZESQ;
	float x = float(ii % CUBEMAPSIZE);
	float y = float(ii / CUBEMAPSIZE);
	float2 st = float2(x,y) / float(CUBEMAPSIZE-1);
	return float4( st, float(face), m.a );
}


#endif

